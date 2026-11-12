Shader "2DFluidSim/ComputeConfinement" {
	Properties {
		_Velocity ("Velocity", 2D) = "black" {}
		_Vorticity ("Vorticity", 2D) = "black" {}
		_Reaction ("Reaction", 2D) = "black" {}
		_Epsilon ("Epsilon", Float) = 1
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

			Texture2D<float4> _Velocity;
			SamplerState sampler_Velocity;
			Texture2D<float4> _Vorticity;
			SamplerState sampler_Vorticity;
			Texture2D<float4> _Reaction;
			SamplerState sampler_Reaction;
			float _Epsilon;
			float _DeltaTime;
			float4 _PixelSize;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 pixel = _PixelSize.xy;
				float left = FluidSimLoad(_Vorticity, sampler_Vorticity, input.uv - float2(pixel.x, 0.0)).r;
				float right = FluidSimLoad(_Vorticity, sampler_Vorticity, input.uv + float2(pixel.x, 0.0)).r;
				float down = FluidSimLoad(_Vorticity, sampler_Vorticity, input.uv - float2(0.0, pixel.y)).r;
				float up = FluidSimLoad(_Vorticity, sampler_Vorticity, input.uv + float2(0.0, pixel.y)).r;
				float2 gradient = float2(right - left, up - down);
				float2 direction = gradient / max(length(gradient), 1e-4);
				float vorticity = FluidSimLoad(_Vorticity, sampler_Vorticity, input.uv).r;
				float2 force = float2(direction.y, -direction.x) * vorticity * _Epsilon;
				float reaction = saturate(dot(FluidSimLoad(_Reaction, sampler_Reaction, input.uv).rgb, float3(1.0, 1.0, 1.0)));
				float2 velocity = FluidSimLoad(_Velocity, sampler_Velocity, input.uv).rg;
				velocity += force * _DeltaTime * (0.25 + reaction);
				return float4(velocity, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
