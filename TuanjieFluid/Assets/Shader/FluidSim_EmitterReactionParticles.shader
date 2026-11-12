Shader "FluidSim/EmitterReactionParticles" {
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_ReactionScale ("Reaction", Vector) = (0,0,0,0)
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
			float4 _ReactionScale;

			float4 frag(FluidSimSpriteOutput input) : SV_TARGET
			{
				FluidSimRequireCapture();
				float mask = _MainTex.Sample(sampler_MainTex, input.uv).a;
				return float4(_ReactionScale.rgb * mask, mask);
			}
			ENDHLSL
		}
	}
}
