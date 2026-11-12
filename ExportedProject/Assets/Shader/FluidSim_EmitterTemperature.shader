Shader "FluidSim/EmitterTemperature" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _TemperatureScale ("Temperature", Float) = 0
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
			float _TemperatureScale;

			float4 frag(FluidSimSpriteOutput input) : SV_TARGET
			{
				float mask = _MainTex.Sample(sampler_MainTex, input.uv).a;
				return float4(_TemperatureScale * mask, 0.0, 0.0, mask);
			}
			ENDHLSL
		}
	}
}
