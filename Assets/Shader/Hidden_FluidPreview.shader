Shader "Hidden/FluidPreview"
{
	SubShader
	{
		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _Reaction;
			sampler2D _Density;
			float4 _PreviewST;

			struct v2f
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img input)
			{
				v2f output;
				output.positionCS = UnityObjectToClipPos(input.vertex);
				output.uv = input.texcoord;
				return output;
			}

			float4 frag(v2f input) : SV_Target
			{
				float2 uv = input.uv * _PreviewST.xy + _PreviewST.zw;
				float3 reaction = tex2D(_Reaction, uv).rgb;
				float3 density = tex2D(_Density, uv).rgb;
				float fire = saturate(reaction.r);
				float poison = saturate(reaction.g);
				float frost = saturate(reaction.b);
				float3 color =
					lerp(float3(0.45, 0.05, 0.0), float3(1.0, 0.58, 0.12), saturate(fire * 2.5)) * saturate(fire * 5.0) +
					lerp(float3(0.02, 0.22, 0.04), float3(0.45, 1.0, 0.35), saturate(poison * 2.5)) * saturate(poison * 5.0) +
					lerp(float3(0.04, 0.12, 0.35), float3(0.65, 0.88, 1.0), saturate(frost * 2.5)) * saturate(frost * 5.0);
				color += density;
				float cover = saturate((fire + poison + frost) * 5.0 + dot(abs(density), 1.0));
				color = lerp(float3(0.07, 0.08, 0.09), color, cover);
				return float4(color, 1.0);
			}
			ENDHLSL
		}
	}
}
