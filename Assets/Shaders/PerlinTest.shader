// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/PerlinTest" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_ColorLow("Color", Color) = (0,0,0,0)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_NoiseFrequency("NoiseFrequency", Float) = 1
		_Displacement("Displacement", Float) = 0.1
			_Tess("Tessellation", Range(1,32)) = 4
		_MaxDist("MaxDist", Float) = 25
		_Cutoff("Cutoff", Range(0,1)) = 0.5
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			Cull Back
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tessDistance nolightmap

			#pragma target 3.0
	#include "Tessellation.cginc"

		// hash based 3d value noise
		// function taken from https://www.shadertoy.com/view/XslGRr
		// Created by inigo quilez - iq/2013
		// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

		float hash(float n)
		{
			return frac(sin(n)*43758.5453);
		}

		float noiseIQ(float3 x)
		{
			// The noise function returns a value in the range -1.0f -> 1.0f
			float3 p = floor(x);
			float3 f = frac(x);
			f = f*f*(3.0 - 2.0*f);
			float n = p.x + p.y*57.0 + 113.0*p.z;
			return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
				lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
				lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
					lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
		}

		//sampler2D _MainTex;

		half _NoiseFrequency;
		fixed _Cutoff;

		float finalNoise(float3 x)
		{
			float amount = noiseIQ(x * _NoiseFrequency);

			if (amount < _Cutoff) amount = 0;
			else
				amount -= _Cutoff;

			return amount;
		}

		struct appdata {
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		float _Tess;
		float _MaxDist;

		float4 tessDistance(appdata v0, appdata v1, appdata v2) {
			float minDist = 10.0;
			//float maxDist = 25.0;
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, _MaxDist, _Tess);
		}

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		
		half _Displacement;

		void vert(inout appdata  v) {
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;// + _Time * 10;
			float3 vertexBase = v.vertex.xyz;

			float finNoise = finalNoise(worldPos);

			v.vertex.xyz += v.normal * finalNoise(worldPos) * _Displacement;

			v.normal *= finNoise;
			//v.vertex.xyz += 0;
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _ColorLow;


		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color

			fixed4 c = lerp(_Color, _ColorLow, finalNoise(IN.worldPos)); // tex2D (_MainTex, IN.uv_MainTex) * 
			o.Albedo = c.rgb;


			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
