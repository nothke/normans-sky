// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "SlinDev/Planet"
{
	Properties
	{
		_MainTex("Texture (RGB)", 2D) = "black" {}
	_Color("Color", Color) = (0, 0, 0, 1)
		_AtmoColor("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1)
		_Size("Size", Float) = 0.1
		_Falloff("Falloff", Float) = 5
		_FalloffPlanet("Falloff Planet", Float) = 5
		_Transparency("Transparency", Float) = 15
		_TransparencyPlanet("Transparency Planet", Float) = 1
	}

		SubShader
	{
		Tags{ "LightMode" = "ForwardBase" }
		/*Pass
	{
		Name "PlanetBase"
		Cull Back

		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"
	
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _AtmoColor;
			uniform float _FalloffPlanet;
			uniform float _TransparencyPlanet;
	
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 worldvertpos : TEXCOORD1;
				float2 texcoord : TEXCOORD2;
			};
	
			v2f vert(appdata_base v)
			{
				v2f o;
	
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.normal = mul((float3x3)_Object2World, v.normal);
				o.worldvertpos = mul(_Object2World, v.vertex).xyz;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
	
				return o;
			}
	
			float4 frag(v2f i) : COLOR
			{
				i.normal = normalize(i.normal);
			float3 viewdir = normalize(_WorldSpaceCameraPos - i.worldvertpos);
	
			float4 atmo = _AtmoColor;
			atmo.a = pow(1.0 - saturate(dot(viewdir, i.normal)), _FalloffPlanet);
			atmo.a *= _TransparencyPlanet*_Color;
	
			float4 color = tex2D(_MainTex, i.texcoord)*_Color;
			color.rgb = lerp(color.rgb, atmo.rgb, atmo.a);
	
			return color*dot(_WorldSpaceLightPos0, i.normal);
			}

		ENDCG
	}*/

		Tags{ "LightMode" = "ForwardBase" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass
	{
		Name "FORWARD"
		Cull Front
		Blend SrcAlpha One
		ZWrite Off

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

		uniform float4 _Color;
		uniform float4 _AtmoColor;
		uniform float _Size;
		uniform float _Falloff;
		uniform float _Transparency;

		struct v2f
		{
			float4 pos : SV_POSITION;
			float3 normal : TEXCOORD0;
			float3 worldvertpos : TEXCOORD1;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
	
			v.vertex.xyz += v.normal*_Size;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
			o.worldvertpos = mul(unity_ObjectToWorld, v.vertex);
	
			return o;
		}

		float4 frag(v2f i) : COLOR
		{
			i.normal = normalize(i.normal);
		float3 viewdir = normalize(i.worldvertpos - _WorldSpaceCameraPos);
	
		float4 color = _AtmoColor;
		color.a = dot(viewdir, i.normal);
		color.a *= dot(i.normal, _WorldSpaceLightPos0);
		color.a = saturate(color.a);
		color.a = pow(color.a, _Falloff);
		color.a *= _Transparency;
		return color;
		}

	ENDCG

	}
		}
	
			FallBack "Diffuse"
	}