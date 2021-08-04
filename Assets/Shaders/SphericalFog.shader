// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

/// Originally made by runevision
/// Upgraded by Nothke to support point lights, colored lights, attenuation
/// Probably VERY unoptimized

Shader "FX/Spherical Fog" {
	Properties {
		_FogBaseColor ("Fog Base Color", Color) = (0,0.7,1,1)
		_FogDenseColor ("Fog Dense Color", Color) = (1,1,1,1)
		_InnerRatio ("Inner Ratio", Range (-10, 0.9999)) = 0.5
		_Density ("Density", Range (0.0, 50.0)) = 10.0
		_ColorFalloff ("Color Falloff", Range (0.0, 500.0)) = 16.0
		_SideBleed ("Side Bleed", Range(0.0, 1.0)) = 0.3
		_Mult("Mult", Range(1, 10)) = 1
	}
	 
	Category {
		Tags { "Queue"="Transparent+99" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode" = "ForwardAdd" }
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One
		Cull Front Lighting Off ZWrite Off
		ZTest Always
		SubShader {
			Pass {
				Tags { "LightMode" = "ForwardAdd"}

				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				//#include "Autolight.cginc"

				uniform float4 _LightColor0;
				// color of light source (from "Lighting.cginc")
				uniform float4x4 unity_WorldToLight; // transformation from world to light space (from Autolight.cginc)
				uniform sampler2D _LightTexture0; // cookie alpha texture map (from Autolight.cginc)
				 
				inline float CalcVolumeFogIntensity (
					float3 sphereCenter,
					float sphereRadius,
					float innerRatio,
					float density,
					float3 cameraPosition,
					float3 viewDirection,
					float maxDistance
				) {
					// Local is the cam position in local space of the sphere.
					float3 local = cameraPosition - sphereCenter;
					
					// Calculate ray-sphere intersection
					float fA = dot (viewDirection, viewDirection);
					float fB = 2 * dot (viewDirection, local);
					float fC = dot(local, local) - sphereRadius * sphereRadius;
					float fD = fB * fB - 4 * fA * fC;
					
					// Early out of no intersection.
					if (fD <= 0.0f)
						return 0;
				 	
					float recpTwoA = 0.5 / fA;
					float DSqrt = sqrt (fD);
					// Distance to front of sphere (0 if inside sphere).
					// This is the distance from the camera where sampling should begin.
					float dist = max ((-fB - DSqrt) * recpTwoA, 0);
					// Distance to back of sphere
					float dist2 = max ((-fB + DSqrt) * recpTwoA, 0);
				 	
				 	// Max sampling depth should be minimum of back of sphere or solid surface hit.
					float backDepth = min (maxDistance, dist2);
					// Calculate initial sample dist and distance between samples.
					float sample = dist;
					float step_distance = (backDepth - dist) / 10;
					
					// The step_contribution is a value where 0 means no reduction in clarity and
					// 1 means 100% reduction in clarity.
					// The step_contribution approaches 1 as the sample distance increases.
					float step_contribution = (1 - 1 / pow (2, step_distance)) * density;
					
					// Calculate value at the center needed to make the value be 1 at the desired inner ratio.
					// This high value does not actually produce high density in the center, since it's clamped to 1.
					float centerValue = 1 / (1 - innerRatio);
					
					// Initially there's no fog, which is full clarity.
					float clarity = 1;
					for ( int seg = 0; seg < 10; seg++ )
					{
						float3 position = local + viewDirection * sample;
						float val = saturate (centerValue * (1 - length (position) / sphereRadius));
						float sample_fog_amount = saturate (val * step_contribution);
						clarity *= (1 - sample_fog_amount);
						sample += step_distance;
					}
					return 1 - clarity;
				}
				 
				fixed4 _FogBaseColor;
				fixed4 _FogDenseColor;
				float _InnerRatio;
				float _Density;
				float _ColorFalloff;
				sampler2D _CameraDepthTexture;

				float _Mult;
				uniform float4 FogParam;


				 
				float _SideBleed;

				struct v2f {
					float4 pos : SV_POSITION;
					float3 view : TEXCOORD0;
					float4 projPos : TEXCOORD1;
					fixed3 lightDir : TEXCOORD2;
					half3 worldNormal : TEXCOORD3;

					// position of the vertex (and fragment) in light space
					float4 posWorld : TEXCOORD4;
					float4 posLight : TEXCOORD5;

					// Farfarer
					float3 _LightCoord : TEXCOORD6; // Replace X with next available texcoord in your struct.
				};
				 
				v2f vert (appdata_base v, float3 normal : NORMAL) {
					v2f o;
					//float4 wPos = mul (_Object2World, v.vertex);
					o.posWorld = mul(unity_ObjectToWorld, v.vertex);
					o.pos = UnityObjectToClipPos (v.vertex);
					o.lightDir = ObjSpaceLightDir(v.vertex);
					o.view = o.posWorld.xyz - _WorldSpaceCameraPos;
					o.projPos = ComputeScreenPos (o.pos);
					o.worldNormal = UnityObjectToWorldNormal(normal);

					// For point lights:
					// {
					//float4x4 modelMatrix = _Object2World;
					//float4x4 modelMatrixInverse = _World2Object;

					//o.posWorld = mul(_Object2World, v.vertex);
					o.posLight = mul(unity_WorldToLight, o.posWorld);
				 	// }

				 	// Farfarer
				 	//#ifdef POINT
					o._LightCoord = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex)).xyz; // Replace X with name of your struct... traditionally o it seems.
					//#endif

					// Move projected z to near plane if point is behind near plane.
					float inFrontOf = ( o.pos.z / o.pos.w ) > 0;
					o.pos.z *= inFrontOf;

					return o;
				}
				 
				half4 frag (v2f i) : COLOR {
					half4 color = half4 (1,1,1,1);
					float depth = LinearEyeDepth (UNITY_SAMPLE_DEPTH (tex2Dproj (_CameraDepthTexture, UNITY_PROJ_COORD (i.projPos))));
					float3 viewDir = normalize (i.view);

					// POINT LIGHTS:
					//float3 lightDirection;
					float attenuation;

					//float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					//lightDirection = normalize(vertexToLightSource);

					//float distance = i.posLight.z; // use z coordinate in light space as signed distance
					//attenuation = tex2D(_LightTexture0, float2(distance, distance)).a;
					//attenuation = 1 - distance;// i.posLight.z;

					// Farfarer
					attenuation = tex2D(_LightTexture0, dot(i._LightCoord,i._LightCoord).rr).UNITY_ATTEN_CHANNEL; // Replace X with name of incoming vertex data struct, traditionally i, it seems.

					// texture lookup for attenuation               
					// alternative with linear attenuation: 
					//float distance = length(vertexToLightSource);
					//attenuation = 1.0 / distance;

					
					//fixed3 screwNormals = fixed3( i.worldNormal.x,i.worldNormal.y, i.worldNormal.z);
					half NdotL = dot(i.worldNormal, normalize(i.lightDir)); // normalize(i.lightDir)
					//half NdotL = dot(o.Normal, o.lightDir);
					
					// Calculate fog density.
					// Scale by factor 1000 to avoid precision errors for large volumes.
					float fog = CalcVolumeFogIntensity (
						FogParam.xyz * 0.001,
						FogParam.w * 0.001,
						_InnerRatio,
						_Density * 1000,
						_WorldSpaceCameraPos * 0.001,
						viewDir,
						depth * 0.0006);
					
					// Calculate ratio of dense color.
					float denseColorRatio = pow (fog, _ColorFalloff);
					
					// Set color based on denseness and alpha based on raw calculated fog density.
					color.rgb = lerp(_FogBaseColor.rgb, _FogDenseColor.rgb, denseColorRatio) * _LightColor0.rgb;
					color.a = fog * lerp (_FogBaseColor.a, _FogDenseColor.a, denseColorRatio) * attenuation;

					//debug
					//color.rgb = normalize(i.lightDir);
					//color.rgb = attenuation;
					//color.rgb = i.posWorld.xyz;
					//color.a = attenuation;
					//color.a = 1;

					color.a *= _SideBleed + NdotL * (1 - _SideBleed) * 2 * _Mult;//attenuation;
					return color;
				}
				ENDCG
			}
		}
	}
	Fallback "VertexLit"
}
