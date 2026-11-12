Shader "2DFluidSim/ComputePressure" {
	Properties {
		_Pressure ("Pressure", 2D) = "black" {}
		_Divergence ("Divergence", 2D) = "black" {}
		_Obstacle ("Obstacle", 2D) = "black" {}
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

			Texture2D<float4> _Pressure;
			SamplerState sampler_Pressure;
			Texture2D<float4> _Divergence;
			SamplerState sampler_Divergence;
			Texture2D<float4> _Obstacle;
			SamplerState sampler_Obstacle;
			float4 _PixelSize;

			float SamplePressure(float2 uv, float center, float blockedCenter)
			{
				float blocked = step(0.1, FluidSimLoad(_Obstacle, sampler_Obstacle, uv).r);
				float pressure = FluidSimLoad(_Pressure, sampler_Pressure, uv).r;
				return lerp(pressure, center, max(blocked, blockedCenter));
			}

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 pixel = _PixelSize.xy;
				float blocked = step(0.1, FluidSimLoad(_Obstacle, sampler_Obstacle, input.uv).r);
				float center = FluidSimLoad(_Pressure, sampler_Pressure, input.uv).r;
				float left = SamplePressure(input.uv - float2(pixel.x, 0.0), center, blocked);
				float right = SamplePressure(input.uv + float2(pixel.x, 0.0), center, blocked);
				float down = SamplePressure(input.uv - float2(0.0, pixel.y), center, blocked);
				float up = SamplePressure(input.uv + float2(0.0, pixel.y), center, blocked);
				float divergence = FluidSimLoad(_Divergence, sampler_Divergence, input.uv).r;
				float pressure = (left + right + down + up - divergence) * 0.25;
				pressure *= 1.0 - blocked;
				return float4(pressure, 0.0, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
