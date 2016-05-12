Shader "Custom/StandardTessTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	_BumpMap("Normal (Normal)", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
			_EdgeLength("Edge length", Range(2,50)) = 15
			_Displacement("Displacement", Range(0, 1.0)) = 0.3
			_HeightMap("Height", 2D) = "black" {}
		_NormalStr("Normal Strength", Float) = 1
			_HeightTexSize("Height Tex Size", Vector) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:disp tessellate:tessEdge nolightmap

		#pragma target 5.0
#include "Tessellation.cginc"

			
		struct appdata {
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		float _EdgeLength;

			float4 tessEdge(appdata v0, appdata v1, appdata v2)
			{
				return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _EdgeLength) ;
			}

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _HeightMap;
		half _Displacement;


		
		void disp(inout appdata v)
		{
			//float d = tex2Dlod(_DispTex, float4(v.texcoord.xy, 0, 0)).r * _Displacement;
			//v.vertex.xyz += sin(v.vertex.xyz) * v.normal * 1;
			v.vertex.xyz += v.normal * tex2Dlod(_HeightMap, float4(v.texcoord.xy, 0, 0)).r * _Displacement;
		}

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		half _NormalStr;
		float2 _HeightTexSize;
		
		float3 FindNormal(float2 uv, float2 u)
		{
			float ht0 = tex2D(_HeightMap, uv + float2(-u.x, 0));
			float ht1 = tex2D(_HeightMap, uv + float2(u.x, 0));
			float ht2 = tex2D(_HeightMap, uv + float2(0, -u.y));
			float ht3 = tex2D(_HeightMap, uv + float2(0, u.y));

			float3 va = normalize(float3(float2(_NormalStr, 0.0), ht1 - ht0));
			float3 vb = normalize(float3(float2(0.0, _NormalStr), ht3 - ht2));

			return cross(va, vb);
		}

		/*
		float3 FindNormal2(float2 uv, float2 heightDim)
		{
			float3 normal = UnpackNormal(tex2D(_BumpMap, uv));

			float me = tex2D(_HeightMap, uv).x;
			float n = tex2D(_HeightMap, float2(uv.x, uv.y + 1.0 / heightDim.y)).x;
			float s = tex2D(_HeightMap, float2(uv.x, uv.y - 1.0 / heightDim.y)).x;
			float e = tex2D(_HeightMap, float2(uv.x - 1.0 / heightDim.x, uv.y)).x;
			float w = tex2D(_HeightMap, float2(uv.x + 1.0 / heightDim.x, uv.y)).x;

			float3 norm = normal;//float3(0.5, 0.5, 0.5);
			float3 temp = norm; //a temporary vector that is not parallel to norm
			if (norm.x == 1)
				temp.y += 0.5;
			else
				temp.x += 0.5;

			//form a basis with norm being one of the axes:
			float3 perp1 = normalize(cross(norm, temp));
			float3 perp2 = normalize(cross(norm, perp1));

			//use the basis to move the normal in its own space by the offset
			float3 normalOffset = -_NormalStr * (((n - me) - (s - me)) * perp1 + ((e - me) - (w - me)) * perp2);
			norm += normalOffset;
			return normalize(norm);
		}
		*/

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			
			

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			
			o.Normal = FindNormal(IN.uv_MainTex, 1.0 / _HeightTexSize);
			//o.Normal = FindNormal2(IN.uv_MainTex, _HeightTexSize);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
