Shader "2DFluidSim/ShiftTexture" {
	Properties {
		_Read ("Read", 2D) = "black" {}
		_Shift ("Shift", Vector) = (0,0,0,0)
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
			float4 _Shift;
			float4 _PixelSize;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float2 uv = input.uv + _Shift.xy * _PixelSize.xy;
				if (uv.x < 0.0 || uv.y < 0.0 || uv.x > 1.0 || uv.y > 1.0)
				{
					return float4(0.0, 0.0, 0.0, 0.0);
				}
				return FluidSimLoad(_Read, sampler_Read, uv);
			}
			ENDHLSL
		}
	}
}
