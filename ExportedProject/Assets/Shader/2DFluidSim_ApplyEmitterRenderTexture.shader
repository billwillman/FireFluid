Shader "2DFluidSim/ApplyEmitterRenderTexture" {
	Properties {
		_EmitterTexture ("Emitter", 2D) = "black" {}
		_Read ("Read", 2D) = "black" {}
		_Offset ("Offset", Vector) = (0,0,0,0)
		_Scaling ("Scaling", Float) = 1
		_Mode ("Mode", Float) = 0
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

			Texture2D<float4> _EmitterTexture;
			SamplerState sampler_EmitterTexture;
			Texture2D<float4> _Read;
			SamplerState sampler_Read;
			float4 _Offset;
			float _Scaling;
			float _Mode;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float4 source = FluidSimLoad(_Read, sampler_Read, input.uv);
				float4 emitted = FluidSimLoad(_EmitterTexture, sampler_EmitterTexture, input.uv);
				float3 delta = (emitted.rgb + _Offset.rgb) * _Scaling;
				if (_Mode < 0.5)
				{
					source.rgb += delta;
					return source;
				}

				// Offset marks a signed field (velocity is stored around 0.5). Those add.
				// Unsigned fields (reaction) keep the stronger value so a source holds its level.
				float signedField = step(0.01, dot(abs(_Offset.rgb), float3(1.0, 1.0, 1.0)));
				source.rgb = lerp(max(source.rgb, delta), source.rgb + delta, signedField);
				return source;
			}
			ENDHLSL
		}
	}
}
