Shader "Custom/TimeBubble_Refraction" {
	Properties {
		_Color ("Tint", Vector) = (1,1,1,1)
		_DeepColor ("Deep Color", Vector) = (1,1,1,1)
		_ScrollTexture ("Scroll Texture", 2D) = "white" {}
		_ScrollSpeed ("Scroll Speed", Float) = 1
		_FresnelPower ("Fresnel Power", Float) = 2
		_DistortionStrength ("Distortion Strength", Float) = 0.05
		_Cutout ("Cutout", Range(0, 1)) = 0.4
		_IntersectionColor ("Intersection Color", Vector) = (1,1,1,1)
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
}