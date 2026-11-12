Shader "2DFluidSim/FluidRender" {
	Properties {
		_ReactionGradientRed ("ReactionGradientRed", 2D) = "red" {}
		_ReactionGradientGreen ("ReactionGradientGreen", 2D) = "green" {}
		_ReactionGradientBlue ("ReactionGradientBlue", 2D) = "blue" {}
		_Density ("Density", 2D) = "black" {}
		_Reaction ("Reaction", 2D) = "black" {}
		_Temperature ("Temperature", 2D) = "black" {}
		_DetailNoise ("Detail Noise", 2D) = "gray" {}
		_LightMap ("Light Map", 2D) = "black" {}
		_DensityAbsorption ("DensityAbsorption", Float) = 1
		_ReactionAbsorption ("ReactionAbsorption", Float) = 10
		_AddNoise ("AddNoise", Range(0, 2)) = 0
		_AlphaBlend ("AlphaBlend", Float) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex FluidSimBlitVert
			#pragma fragment frag
			#include "FluidSimBlit.hlsl"

			Texture2D<float4> _ReactionGradientRed;
			SamplerState sampler_ReactionGradientRed;
			Texture2D<float4> _ReactionGradientGreen;
			SamplerState sampler_ReactionGradientGreen;
			Texture2D<float4> _ReactionGradientBlue;
			SamplerState sampler_ReactionGradientBlue;
			Texture2D<float4> _Density;
			SamplerState sampler_Density;
			Texture2D<float4> _Reaction;
			SamplerState sampler_Reaction;
			Texture2D<float4> _DetailNoise;
			SamplerState sampler_DetailNoise;
			Texture2D<float4> _LightMap;
			SamplerState sampler_LightMap;
			float _DensityAbsorption;
			float _ReactionAbsorption;
			float _AddNoise;
			float _AlphaBlend;

			float3 Gradient(Texture2D<float4> tex, SamplerState samp, float amount)
			{
				return tex.SampleLevel(samp, float2(saturate(amount), 0.5), 0).rgb;
			}

			float4 frag(FluidSimBlitOutput input) : SV_TARGET
			{
				float3 reaction = FluidSimLoad(_Reaction, sampler_Reaction, input.uv).rgb;
				float detail = FluidSimLoad(_DetailNoise, sampler_DetailNoise, input.uv * 8.0).r;
				reaction = saturate(reaction + (detail - 0.5) * _AddNoise);
				float3 density = FluidSimLoad(_Density, sampler_Density, input.uv).rgb;
				float3 color =
					Gradient(_ReactionGradientRed, sampler_ReactionGradientRed, reaction.r) * reaction.r +
					Gradient(_ReactionGradientGreen, sampler_ReactionGradientGreen, reaction.g) * reaction.g +
					Gradient(_ReactionGradientBlue, sampler_ReactionGradientBlue, reaction.b) * reaction.b;
				color += density;
				color += FluidSimLoad(_LightMap, sampler_LightMap, input.uv).rgb;
				float alpha = saturate(dot(reaction, 1.0) * _ReactionAbsorption * 0.1 + dot(density, 1.0) * _DensityAbsorption * 0.05);
				alpha = lerp(1.0, alpha, saturate(_AlphaBlend));
				return float4(color, alpha);
			}
			ENDHLSL
		}
	}
}
