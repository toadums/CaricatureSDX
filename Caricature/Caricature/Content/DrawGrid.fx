/*********************************************************************************************************
 * Author: Paul Demchuk
 * Date: Jan 17, 2013
 * Purpose: Take a texture, render it
 *********************************************************************************************************/
uniform Texture2D InputTexture;
SamplerState Sampler;

int Rows;
int Columns;

uniform float ImageWidth;
uniform float ImageHeight;


float4 PSMain(float2 pos: TEXCOORD, float4 SVP : SV_POSITION) : SV_TARGET {

	float4 image = InputTexture.Sample(Sampler, pos);
	float2 size = float2(ImageWidth,ImageHeight);
	float2 actualPos = size*pos;
	
	if(actualPos.x % Rows == Rows - 1 || actualPos.x % Rows < 2)
		return float4(0,0,0,1);
	else if(actualPos.y % Columns == Columns - 1 || actualPos.y % Columns < 2)
		return float4(0,0,0,1);
	return image;



}

technique  {
	pass {
		Profile = 9.3;
		PixelShader = PSMain;
	}
}

/*
1 0 0   -   0

1 1 0   -   1/4

0 1 0   -   1/2

0 1 1   -   3/4

0 0 1   -   1

1 0 1