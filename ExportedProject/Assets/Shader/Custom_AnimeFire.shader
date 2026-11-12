Shader "Custom/AnimeFire" {
	Properties {
		_Color ("Tint", Vector) = (1,1,1,1)
		_RGradient ("Red Gradient", 2D) = "white" {}
		_GGradient ("Green Gradient", 2D) = "white" {}
		_BGradient ("Blue Gradient", 2D) = "white" {}
		_GradientWeights ("Gradient Weights", Vector) = (1,0,0,0)
		_GradientGlow ("Gradient Glow", Float) = 0
		_Emissive ("Emissive", 2D) = "black" {}
		_EmissiveFactor ("Emissive Factor", Float) = 32
		_EmissionScale ("Emission Scale", Vector) = (1,1,1,1)
		_EmissionScrollSpeed ("Emission Scroll Speed", Float) = 1
		_EmissiveCutout ("Emissive Cutout", Float) = 0.5
		_EmissiveMask ("Emissive Mask", Float) = 0
		_AnimationHeightOffset ("Animation Height Offset", Float) = 0
		_AnimationAmount ("Animation Amount", Range(0, 0.1)) = 0.01
		_SeedAnimationToOrigin ("SeedAnimationToOrigin", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Universal Render Pipeline/Lit"
}