Shader "2DFluidSim/ApplyImpulse" {
	Properties {
		_Density ("Density", 2D) = "black" {}
		_Reaction ("Reaction", 2D) = "black" {}
		_Amount ("Amount", Float) = 0
		_Extinguishment ("Extinguishment", Float) = 0
		_DensityEmittedByR ("Density R", Color) = (0,0,0,0)
		_DensityEmittedByG ("Density G", Color) = (0,0,0,0)
		_DensityEmittedByB ("Density B", Color) = (0,0,0,0)
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

			Texture2D<float4> _Density;
			SamplerState sampler_Density;
			Texture2D<float4> _Reaction;
			SamplerState sampler_Reaction;
			float _Amount;
			float _Extinguishment;
			float4 _DensityEmittedByR;
			float4 _DensityEmittedByG;
			float4 _DensityEmittedByB;

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float4 density = FluidSimLoad(_Density, sampler_Density, input.uv);
				float3 reaction = FluidSimLoad(_Reaction, sampler_Reaction, input.uv).rgb;
				float3 emitted =
					_DensityEmittedByR.rgb * reaction.r +
					_DensityEmittedByG.rgb * reaction.g +
					_DensityEmittedByB.rgb * reaction.b;
				density.rgb += emitted * _Amount;
				float extinguish = saturate(_Extinguishment * max(reaction.g, reaction.b));
				density.rgb *= 1.0 - extinguish;
				density.rgb = max(density.rgb, 0.0);
				return density;
			}
			ENDHLSL
		}
	}
}
