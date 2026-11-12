Shader "FluidSim/EmitterVelocity" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _OutwardsForce ("Outwards Force", Float) = 0
		[PerRendererData] _NoiseForce ("Noise Force", Float) = 0
		[PerRendererData] _VelocityX ("Velocity X", Float) = 0
		[PerRendererData] _VelocityY ("Velocity Y", Float) = 0
		[PerRendererData] _RotationalForce ("Rotational Force", Float) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		Cull Off
		ZWrite Off
		ZTest Always
		Blend One One

		Pass {
			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex FluidSimSpriteVert
			#pragma fragment frag
			#include "FluidSimBlit.hlsl"

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float _OutwardsForce;
			float _NoiseForce;
			float _VelocityX;
			float _VelocityY;
			float _RotationalForce;

			float4 frag(FluidSimSpriteOutput input) : SV_TARGET
			{
				float mask = _MainTex.Sample(sampler_MainTex, input.uv).a;
				float2 fromCenter = input.uv * 2.0 - 1.0;
				float radius = max(length(fromCenter), 1e-4);
				float2 outward = fromCenter / radius;
				float2 tangent = float2(-outward.y, outward.x);
				float2 noise = float2(
					FluidSimHash(input.uv * 40.0 + _Time.y),
					FluidSimHash(input.uv * 40.0 + _Time.y + 19.2)) * 2.0 - 1.0;

				// Sim later multiplies by velocityEmitterScaling (128 in this project) and
				// advects with dt * texel size. Divide so prefab forces stay in range:
				// 0.2 is a drift, 32 is a short blast.
				float2 velocity = float2(_VelocityX, _VelocityY);
				velocity += outward * _OutwardsForce;
				velocity += tangent * _RotationalForce;
				velocity += noise * _NoiseForce;
				velocity *= mask / 32.0;
				return float4(velocity, 0.0, mask);
			}
			ENDHLSL
		}
	}
}
