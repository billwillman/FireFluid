Shader "2DFluidSim/ApplyDiffusion" {
	Properties {
		_Read ("Read", 2D) = "black" {}
		_Diffusion ("Diffusion", Float) = 0
		_DeltaTime ("Delta Time", Float) = 0.1
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
			float _Diffusion;
			float _DeltaTime;
			float4 _PixelSize;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 pixel = _PixelSize.xy;
				float4 center = FluidSimLoad(_Read, sampler_Read, input.uv);
				float4 left = FluidSimLoad(_Read, sampler_Read, input.uv - float2(pixel.x, 0.0));
				float4 right = FluidSimLoad(_Read, sampler_Read, input.uv + float2(pixel.x, 0.0));
				float4 down = FluidSimLoad(_Read, sampler_Read, input.uv - float2(0.0, pixel.y));
				float4 up = FluidSimLoad(_Read, sampler_Read, input.uv + float2(0.0, pixel.y));
				float alpha = _Diffusion * _DeltaTime;
				return (center + alpha * (left + right + down + up)) / (1.0 + 4.0 * alpha);
			}
			ENDHLSL
		}
	}
}
