Shader "2DFluidSim/ComputeVorticity" {
	Properties {
		_Velocity ("Velocity", 2D) = "black" {}
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

			Texture2D<float4> _Velocity;
			SamplerState sampler_Velocity;
			float4 _PixelSize;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 pixel = _PixelSize.xy;
				float2 left = FluidSimLoad(_Velocity, sampler_Velocity, input.uv - float2(pixel.x, 0.0)).rg;
				float2 right = FluidSimLoad(_Velocity, sampler_Velocity, input.uv + float2(pixel.x, 0.0)).rg;
				float2 down = FluidSimLoad(_Velocity, sampler_Velocity, input.uv - float2(0.0, pixel.y)).rg;
				float2 up = FluidSimLoad(_Velocity, sampler_Velocity, input.uv + float2(0.0, pixel.y)).rg;
				float vorticity = 0.5 * ((right.y - left.y) - (up.x - down.x));
				return float4(vorticity, 0.0, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
