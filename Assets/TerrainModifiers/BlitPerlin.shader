Shader "Hidden/BlitPerlin"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	_NoiseFrequency("NoiseFrequency", Float) = 1
		_Offset ("Offset", Vector) = (0,0,0)
		
		_Bias ("Bias", Float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "LeonsPerlin.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			half _NoiseFrequency;
			half3 _Offset;
			fixed _Bias;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 base = tex2D(_MainTex, i.uv);

			float noise = noiseIQ(_Offset + float3(i.uv.x, i.uv.y, 0) * _NoiseFrequency);

			float3 blend = noise;

			//noise = round(noise);

			// overlay function
			//if (col.r < 0.5)
			//	col = 2 * col * noise;
			//else
			//	col = 1 - (2 * (1 - col) * (1 - noise));
			
			// optimized overlay without branching
			half3 br = clamp(sign(base.rgb - 0.5), 0.0, 1.0);
			half3 multiply = 2 * base.rgb * blend;
			half3 screen = 1 - 2 * (1 - base.rgb) *(1 - blend);

			half3 overlay = lerp(multiply, screen, br);
			
			return half4(overlay, 0);
			}
			ENDCG
		}
	}
}
