Shader "2DFluidSim/ApplyBuoyancy" {
	Properties {
		_Temperature ("Temperature", 2D) = "black" {}
		_Velocity ("Velocity", 2D) = "black" {}
		_Density ("Density", 2D) = "black" {}
		_Up ("Up", Vector) = (0,1,0,0)
		_Buoyancy ("Buoyancy", Float) = 1
		_AmbientTemperature ("Ambient Temperature", Float) = 0
		_Weight ("Weight", Float) = 0
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

			Texture2D<float4> _Temperature;
			SamplerState sampler_Temperature;
			Texture2D<float4> _Velocity;
			SamplerState sampler_Velocity;
			Texture2D<float4> _Density;
			SamplerState sampler_Density;
			float4 _Up;
			float _Buoyancy;
			float _AmbientTemperature;
			float _Weight;
			float _DeltaTime;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float temperature = FluidSimLoad(_Temperature, sampler_Temperature, input.uv).r;
				float density = dot(FluidSimLoad(_Density, sampler_Density, input.uv).rgb, float3(0.333, 0.333, 0.333));
				float2 velocity = FluidSimLoad(_Velocity, sampler_Velocity, input.uv).rg;
				float lift = (temperature - _AmbientTemperature) * _Buoyancy - density * _Weight;
				velocity += _Up.xy * lift * _DeltaTime;
				return float4(velocity, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
