Shader "2DFluidSim/ApplyNoise" {
	Properties {
		_Read ("Read", 2D) = "black" {}
		_Intensity ("Intensity", Float) = 0
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
			float _Intensity;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float4 value = FluidSimLoad(_Read, sampler_Read, input.uv);
				float noise = FluidSimHash(input.uv * 128.0 + _Time.y) - 0.5;
				value.r += noise * _Intensity;
				return value;
			}
			ENDHLSL
		}
	}
}
