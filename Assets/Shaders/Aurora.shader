// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Aurora"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveGain("WaveGain", float) = 1
		_WaveRate("WaveRate", float) = 1
		_WaveScale("WaveScale", float) = 1
		_Wave2Gain("Wave 2 Gain", float) = 1
		_Wave2Scale("Wave 2 Scale", float) = 1
		_Wave2Rate("Wave 2 Rate", float) = 1
		_Color("Color", Color) = (1,1,1,1)
		_TopColor("Top Color", Color) = (1,1,1,1)
		_ColorRate("Color Rate", float) = 1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True" "DisableBatching" = "True" }
		ZWrite Off
		Blend One One
		Cull Off
		LOD 100

		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _WaveGain;
			half _WaveRate;
			half _WaveScale;
			half _Wave2Scale;
			half _Wave2Gain;
			half _Wave2Rate;
			half _ColorRate;

			half4 _Color;
			half4 _TopColor;
			
			v2f vert (appdata v)
			{
				v2f o;

				v.vertex.x += sin(v.vertex.x * _WaveScale + _Time * _WaveRate) * _WaveGain;
				v.vertex.y += cos(v.vertex.y * _WaveScale + _Time * _WaveRate) * _WaveGain;

				v.vertex.x += sin(v.vertex.x * _Wave2Scale + _Time * _Wave2Rate) * _Wave2Gain;



				//v.vertex.y += sin(_Time * v.vertex.y * _WaveRate) * _WaveGain;

				//float4 vobj = mul(_World2Object, v.vertex);

				//vobj.y += sin(_Time * vobj.x * _WaveRate) * _WaveGain;
				//vobj.x += sin(_Time * vobj.y * _WaveRate) * _WaveGain;

				//vobj.x = 0;

				//v.vertex.y = 0;

				//o.vertex = mul(UNITY_MATRIX_VP, vobj);



				o.vertex = UnityObjectToClipPos(v.vertex );
				o.color = sin(v.vertex.y * _ColorRate);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

			col.rgb *= lerp(_Color, _TopColor, i.uv.y);
			
			col.a = tex2D(_MainTex, i.uv).a;

			// blending in frag (more expensive)
			//half4 worldpos = mul(_Object2World, i.vertex);
			//half a = sin(worldpos.x * 50);
			//col *= 0.6 + a;

			col *= 0.5 + i.color;

			// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
			ENDCG
		}
	}
}
