Shader "Custom/TransparentFresnelEmissive"
{
	Properties
	{
		_InnerColor("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		_FauxLightColor("Faux Light Color", Color) = (0.26,0.19,0.16,0.0)
		_FauxLight("Faux Light", Vector) = (0,1,0,0)
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Cull Back
		Blend One One

		CGPROGRAM
#pragma surface surf Lambert

	struct Input
	{
		float3 viewDir;
	};

	float4 _InnerColor;
	float4 _RimColor;
	float _RimPower;
	float4 _FauxLight;
	float4 _FauxLightColor;

	void surf(Input IN, inout SurfaceOutput o)
	{
		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		o.Emission = 20* _InnerColor.rgb * _RimColor.rgb * pow(rim, _RimPower);

		half NdotL = dot(o.Normal, _FauxLight);

		o.Emission += NdotL * _FauxLightColor.rgb;
	}
	ENDCG
	}
		Fallback "Diffuse"
}