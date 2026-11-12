Shader "2DFluidSim/ApplyAdvection" {
	Properties {
		_Read ("Read", 2D) = "black" {}
		_Velocity ("Velocity", 2D) = "black" {}
		_Obstacle ("Obstacle", 2D) = "black" {}
		_Dissipate ("Dissipate", Vector) = (1,1,1,1)
		_Decay ("Decay", Vector) = (0,0,0,0)
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
			Texture2D<float4> _Velocity;
			SamplerState sampler_Velocity;
			Texture2D<float4> _Obstacle;
			SamplerState sampler_Obstacle;
			float4 _Dissipate;
			float4 _Decay;
			float _DeltaTime;
			float4 _PixelSize;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 velocity = FluidSimLoad(_Velocity, sampler_Velocity, input.uv).rg;
				float2 previous = input.uv - velocity * _DeltaTime * _PixelSize.xy;
				float4 value = FluidSimLoad(_Read, sampler_Read, previous);
				float blocked = step(0.1, FluidSimLoad(_Obstacle, sampler_Obstacle, input.uv).r);
				float4 stayed = FluidSimLoad(_Read, sampler_Read, input.uv);
				value = lerp(value, stayed, blocked);
				return value * _Dissipate - _Decay;
			}
			ENDHLSL
		}
	}
}
