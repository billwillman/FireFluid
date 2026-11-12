#ifndef FLUIDSIM_BLIT_INCLUDED
#define FLUIDSIM_BLIT_INCLUDED

#include "UnityCG.cginc"

float _FluidCapture;

void FluidSimRequireCapture()
{
	clip(_FluidCapture - 0.5);
}

struct FluidSimBlitInput
{
	float4 positionOS : POSITION;
	float2 uv : TEXCOORD0;
};

struct FluidSimBlitOutput
{
	float4 positionCS : SV_POSITION;
	float2 uv : TEXCOORD0;
};

struct FluidSimSpriteInput
{
	float4 positionOS : POSITION;
	float2 uv : TEXCOORD0;
	float4 color : COLOR;
};

struct FluidSimSpriteOutput
{
	float4 positionCS : SV_POSITION;
	float2 uv : TEXCOORD0;
	float4 color : COLOR;
};

FluidSimBlitOutput FluidSimBlitVert(FluidSimBlitInput input)
{
	FluidSimBlitOutput output;
	output.positionCS = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(input.positionOS.xyz, 1.0)));
	output.uv = input.uv;
	return output;
}

FluidSimSpriteOutput FluidSimSpriteVert(FluidSimSpriteInput input)
{
	FluidSimSpriteOutput output;
	output.positionCS = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(input.positionOS.xyz, 1.0)));
	output.uv = input.uv;
	output.color = input.color;
	return output;
}

float FluidSimHash(float2 p)
{
	return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

float4 FluidSimLoad(Texture2D<float4> tex, SamplerState samp, float2 uv)
{
	return tex.SampleLevel(samp, saturate(uv), 0);
}

float4 FluidSimLoadLevel(Texture2D<float4> tex, SamplerState samp, float2 uv, float level)
{
	return tex.SampleLevel(samp, saturate(uv), level);
}

#endif
