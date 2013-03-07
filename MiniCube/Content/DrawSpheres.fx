/*********************************************************************************************************
 * Author: Paul Demchuk
 * Date: Jan 17, 2013
 * Purpose: Simply take two textures and combine them
 *********************************************************************************************************/

float Cols;
float Rows;

float4 PSMain(float2 pos: TEXCOORD, float4 SVP : SV_POSITION) : SV_TARGET {

	float x = 1.0f / Rows;
	float y = 1.0f / Cols;
	
	float3 pixel = float3(0,0,0);

	bool needBreak = false;

	[unroll(6)]
	for(int i = 0; i <= Rows; i ++){
		[unroll(6)]
		for(int j = 0; j <= Cols; j++){
			//if(pos.x >= x * i - 0.1 && pos.x <= x * i + 0.1 &&
				//pos.y >= y * j - 0.1 && pos.y <= y * j + 0.1){
				
			if(length(pos - float2(x*i, y*j)) < 0.05){
				pixel = float3(1,0,0);

				needBreak = true;
				break;
			}
		}
		if(needBreak) break;
	}

	return float4(pixel, 1);

}

technique  {
	pass {
		Profile = 9.3;
		PixelShader = PSMain;
	}
}