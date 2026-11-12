Shader "2DFluidSim/ComputeProjection" {
	Properties {
		_Pressure ("Pressure", 2D) = "black" {}
		_Velocity ("Velocity", 2D) = "black" {}
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
			Texture2D<float4> _Velocity;
			SamplerState sampler_Velocity;
			Texture2D<float4> _Obstacle;
			SamplerState sampler_Obstacle;
			float4 _PixelSize;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 pixel = _PixelSize.xy;
				float blocked = step(0.1, FluidSimLoad(_Obstacle, sampler_Obstacle, input.uv).r);
				float left = FluidSimLoad(_Pressure, sampler_Pressure, input.uv - float2(pixel.x, 0.0)).r;
				float right = FluidSimLoad(_Pressure, sampler_Pressure, input.uv + float2(pixel.x, 0.0)).r;
				float down = FluidSimLoad(_Pressure, sampler_Pressure, input.uv - float2(0.0, pixel.y)).r;
				float up = FluidSimLoad(_Pressure, sampler_Pressure, input.uv + float2(0.0, pixel.y)).r;
				float2 velocity = FluidSimLoad(_Velocity, sampler_Velocity, input.uv).rg;
				velocity.x -= 0.5 * (right - left);
				velocity.y -= 0.5 * (up - down);
				velocity *= 1.0 - blocked;
				return float4(velocity, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
