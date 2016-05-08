Shader "Custom/Planet" {
	Properties{
		_MainTex("Texture (RGB)", 2D) = "black" {}
	_Color("Color", Color) = (0, 0, 0, 1)
		_AtmoColor("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1)
		_Size("Atmosphere Size", Range(0, 5)) = 0.12
		_Falloff("Falloff", Float) = 2
		_Transparency("Transparency", Float) = 1
	}

		SubShader{
		Pass{
		Name "AtmosphereBase"
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		uniform float4 _Color;
	uniform float4 _AtmoColor;
	uniform float _Size;
	uniform float _Falloff;
	uniform float _Transparency;

	struct v2f {
		float4 pos : SV_POSITION;
		float3 normal : TEXCOORD0;
		float2 atmofalloff : TEXCOORD1;
	};

	v2f vert(appdata_base v) {
		v2f o;
		v.vertex.xyz += v.normal *     _Size;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.normal = v.normal;
		float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		o.atmofalloff = float2(pow(saturate(dot(viewDir, v.normal)), _Falloff), 0.5);
		return o;
	}

	float4 frag(v2f i) : COLOR{
		i.normal = normalize(i.normal);
	float4 color = _AtmoColor;
	color.a = i.atmofalloff.x * _Transparency * _Color;
	return color;
	}
		ENDCG
	}

		// *** surface shader to render the planet itself ***
		Name "PlanetBase"
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0

		uniform sampler2D _MainTex;
	uniform float4 _Color;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
		// *** surface shader end ***
	}
		FallBack "Diffuse"
}