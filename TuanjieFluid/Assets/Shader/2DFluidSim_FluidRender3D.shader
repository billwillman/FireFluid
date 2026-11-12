Shader "2DFluidSim/FluidRender3D" {
	Properties {
		_ReactionGradientRed ("ReactionGradientRed", 2D) = "red" {}
		_ReactionGradientGreen ("ReactionGradientGreen", 2D) = "green" {}
		_ReactionGradientBlue ("ReactionGradientBlue", 2D) = "blue" {}
		_Density ("Density", 2D) = "black" {}
		_Reaction ("Reaction", 2D) = "black" {}
		_Temperature ("Temperature", 2D) = "black" {}
		_Velocity ("Velocity", 2D) = "black" {}
		_DetailNoise ("Detail Noise", 2D) = "gray" {}
		_LightMap ("Light Map", 2D) = "black" {}
		_Pressure ("Pressure", 2D) = "black" {}
		_DensityAbsorption ("DensityAbsorption", Float) = 1
		_ReactionAbsorption ("ReactionAbsorption", Float) = 10
		_ReactionPower ("Reaction Power", Float) = 1
		_ParllaxFactor ("ParallaxFactor", Float) = 0.1
		_ParallaxEllevationReaction ("ParallaxEllevationReaction", Float) = 32
		_TemperatureAbsorption ("_TemperatureAbsorption", Float) = 1
		_ColdFogColor ("Cold Fog Color", Vector) = (1,1,1,1)
		_FogOfWarColor ("Fog Of War Color", Vector) = (0,0,0,1)
		_ReactionBloom ("ReactionBloom", Float) = 1
		_DetailNoiseFactorReaction ("Detail Noise Factor Reaction", Float) = 8
		_DetailNoiseFactorFog ("Detail Noise Factor Fog", Float) = 8
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex vert
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
			Texture2D<float4> _Temperature;
			SamplerState sampler_Temperature;
			Texture2D<float4> _DetailNoise;
			SamplerState sampler_DetailNoise;
			Texture2D<float4> _LightMap;
			SamplerState sampler_LightMap;
			float _DensityAbsorption;
			float _ReactionAbsorption;
			float _ReactionPower;
			float _ParllaxFactor;
			float _ParallaxEllevationReaction;
			float _TemperatureAbsorption;
			float4 _ColdFogColor;
			float4 _FogOfWarColor;
			float _ReactionBloom;
			float _DetailNoiseFactorReaction;
			float _DetailNoiseFactorFog;

			struct RenderInput
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct RenderOutput
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 world : TEXCOORD1;
			};

			RenderOutput vert(RenderInput input)
			{
				RenderOutput output;
				float3 world = mul(unity_ObjectToWorld, float4(input.positionOS.xyz, 1.0)).xyz;
				output.positionCS = mul(unity_MatrixVP, float4(world, 1.0));
				output.uv = input.uv;
				output.world = world;
				return output;
			}

			float3 Gradient(Texture2D<float4> tex, SamplerState samp, float amount)
			{
				return tex.SampleLevel(samp, float2(saturate(amount), 0.5), 0).rgb;
			}

			float4 frag(RenderOutput input) : SV_TARGET
			{
				float3 view = normalize(_WorldSpaceCameraPos.xyz - input.world);
				float2 parallax = view.xy * _ParllaxFactor * _ParallaxEllevationReaction;
				float2 uv = input.uv + parallax * 0.02;
				float3 reaction = FluidSimLoad(_Reaction, sampler_Reaction, uv).rgb;
				float detailScale = max(_DetailNoiseFactorReaction, 1.0);
				float detail = FluidSimLoad(_DetailNoise, sampler_DetailNoise, input.uv * detailScale).r;
				reaction *= lerp(0.85, 1.15, saturate(detail));
				float fire = saturate(reaction.r);
				float poison = saturate(reaction.g);
				float frost = saturate(reaction.b);
				float3 density = FluidSimLoad(_Density, sampler_Density, uv).rgb;
				float temperature = FluidSimLoad(_Temperature, sampler_Temperature, input.uv).r;
				float3 color =
					lerp(float3(0.45, 0.05, 0.0), float3(1.0, 0.58, 0.12), saturate(fire * 2.5)) * saturate(fire * 5.0) +
					lerp(float3(0.02, 0.22, 0.04), float3(0.45, 1.0, 0.35), saturate(poison * 2.5)) * saturate(poison * 5.0) +
					lerp(float3(0.04, 0.12, 0.35), float3(0.65, 0.88, 1.0), saturate(frost * 2.5)) * saturate(frost * 5.0);
				color += density;
				color += FluidSimLoad(_LightMap, sampler_LightMap, input.uv).rgb * _ReactionBloom;
				float cold = saturate(-temperature) * _TemperatureAbsorption * 0.05;
				color = lerp(color, _ColdFogColor.rgb, cold);
				float edge = saturate(detail);
				color = lerp(color, _FogOfWarColor.rgb, _FogOfWarColor.a * (1.0 - edge) * 0.15);
				float alpha = saturate((fire + poison + frost) * 5.0 + dot(abs(density), 1.0) * _DensityAbsorption * 0.35);
				return float4(color, alpha);
			}
			ENDHLSL
		}
	}
}
