Shader "2DFluidSim/ApplyObstacleBorder" {
	Properties {
		_Read ("Read", 2D) = "black" {}
		_PixelSize ("Pixel Size", Vector) = (0.01,0.01,0,0)
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

			Texture2D<float4> _Read;
			SamplerState sampler_Read;
			float4 _PixelSize;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 pixel = _PixelSize.xy;
				float center = FluidSimLoad(_Read, sampler_Read, input.uv).r;
				float left = FluidSimLoad(_Read, sampler_Read, input.uv - float2(pixel.x, 0.0)).r;
				float right = FluidSimLoad(_Read, sampler_Read, input.uv + float2(pixel.x, 0.0)).r;
				float down = FluidSimLoad(_Read, sampler_Read, input.uv - float2(0.0, pixel.y)).r;
				float up = FluidSimLoad(_Read, sampler_Read, input.uv + float2(0.0, pixel.y)).r;
				float dilated = max(center, max(max(left, right), max(down, up)));
				return float4(dilated, dilated, dilated, 1.0);
			}
			ENDHLSL
		}
	}
}
