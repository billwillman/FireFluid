Shader "2DFluidSim/ComputeLight" {
	Properties {
		_Reaction ("Reaction", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Off
		ZWrite Off
		ZTest Always

		Pass {
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex FluidSimBlitVert
			#pragma fragment frag
			#include "FluidSimBlit.hlsl"

			Texture2D<float4> _Reaction;
			SamplerState sampler_Reaction;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 texel = float2(max(abs(ddx(input.uv.x)), 1.0 / 64.0), max(abs(ddy(input.uv.y)), 1.0 / 64.0));
				float4 center = FluidSimLoad(_Reaction, sampler_Reaction, input.uv);
				float4 left = FluidSimLoad(_Reaction, sampler_Reaction, input.uv - float2(texel.x, 0.0));
				float4 right = FluidSimLoad(_Reaction, sampler_Reaction, input.uv + float2(texel.x, 0.0));
				float4 down = FluidSimLoad(_Reaction, sampler_Reaction, input.uv - float2(0.0, texel.y));
				float4 up = FluidSimLoad(_Reaction, sampler_Reaction, input.uv + float2(0.0, texel.y));
				return (center + left + right + down + up) * 0.2;
			}
			ENDHLSL
		}
	}
}
