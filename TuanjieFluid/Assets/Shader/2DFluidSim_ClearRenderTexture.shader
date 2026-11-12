Shader "2DFluidSim/ClearRenderTexture" {
	Properties {
		_ClearColor ("Clear Color", Color) = (0,0,0,0)
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

			float4 _ClearColor;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				return _ClearColor;
			}
			ENDHLSL
		}
	}
}
