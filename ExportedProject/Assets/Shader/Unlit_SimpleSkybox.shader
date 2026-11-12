Shader "Unlit/SimpleSkybox" {
	Properties {
		_Clouds ("Clouds", 2D) = "white" {}
		_Cloudiness ("Cloudiness", Range(0, 32)) = 1
		_CloudUVProjection ("CloudProjection Planar/Hemisphere", Range(0, 1)) = 0.5
		_CloudMovementSpeed ("Cloud Movement Speed", Float) = 0.5
		_CloudMovementOffset ("Cloud Movement Offset", Float) = 0.5
		_HorizonCloudFactor ("Horizon Cloud Factor", Range(0, 16)) = 0
		_SunColor ("_SunColor", Vector) = (1,1,1,1)
		_SkyColor ("_SkyColor", Vector) = (0.5,0.5,1,1)
		_GroundColor ("_GroundColor", Vector) = (0.5,0.3,0.2,1)
		_GroundLevel ("_GroundLevel", Range(-1, 1)) = 0
		_SunDir ("_SunDir", Vector) = (0,1,0,0)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
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

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
}