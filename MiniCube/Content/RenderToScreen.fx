/*********************************************************************************************************
 * Author: Paul Demchuk
 * Date: Jan 17, 2013
 * Purpose: Simply take two textures and combine them
 *********************************************************************************************************/

Texture2D Tex1, Tex2;
SamplerState Sampler1, Sampler2;

float4 PSMain(float2 pos: TEXCOORD, float4 SVP : SV_POSITION) : SV_TARGET {

	float r = Tex2.Sample(Sampler2, pos).r;
	float3 pixel = Tex1.Sample(Sampler1, pos).rgb;

	if(r > 0)
		return float4(1,1,1,1);
	else
		return float4(pixel,1);

}

technique  {
	pass {
		Profile = 9.3;
		PixelShader = PSMain;
	}
}