// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/TransparentFresnel"
{
	Properties
	{
		_InnerColor("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		_FadeDistance("Fade Start Distance", float) = 0.5
		_Alpha ("Alpha", float) = 0.5
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Cull Back
		Blend One One

		CGPROGRAM
#pragma surface surf Lambert vertex:vert nolightmap
#include "UnityCG.cginc"

	struct Input
	{
		float3 viewDir;
		float dist;
	};

	float4 _InnerColor;
	float4 _RimColor;
	float _RimPower;
	float _FadeDistance;
	float _Alpha;


	void vert(inout appdata_base v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);

		float dist = _FadeDistance * (-100 + distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex)));
		
		o.dist = dist;
		//o.color = 1;
		//v.vertex.xyz += 1;
	}

	void surf(Input IN, inout SurfaceOutput o)
	{
		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

		half dist = clamp(IN.dist, 0, 1);

		o.Albedo = 20 * _InnerColor.rgb * _RimColor.rgb * pow(rim, _RimPower) * dist;

		o.Alpha = _Alpha;
		//o.Alpha = IN.dist;
		//o.Emission = _RimColor.rgb * pow(rim, _RimPower);
	}
	ENDCG
	}
		Fallback "Diffuse"
}