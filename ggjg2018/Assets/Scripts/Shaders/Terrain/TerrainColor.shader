Shader "Stephen/Terrain/ColorShading"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorMult ("Texture Color", Color) = (0.5, 0.5, 0.5, 0.5)

		_ShinyRamp("Shiny Ramp (RGB)", 2D) = "black" {}
		[HDR]_ShinyColor("Shiny Color", Color) = (1,1,1,1)
		_ShinySpeed("Shiny Speed", Float) = 8.0
		_ShinyPosSize("Shiny Scale", Float) = 1.0

		_CloudTex ("Cloud Texture", 2D) = "grey" {}
		_CloudCutoff("Cloud Cutoff", Range(0,1)) = 0.0
	}
	Category
	{
		Tags { "Queue"="Transparent" "RenderType"="Opaque"}
		// "DisableBatching" = "true"

		SubShader
		{
			Blend DstColor Zero
			ZWrite Off
			//LOD 100
			Offset -30, 0


			Pass
			{
				CGPROGRAM
				#pragma vertex vertTerrain
				#pragma fragment fragTerrain

				#pragma shader_feature _SHINY_RAMP
				#pragma shader_feature _SHINY_RAMP_WORLD_POS
				#pragma shader_feature _SHINY_RAMP_OFFSET
				#pragma shader_feature _SHINY_RAMP_COLOR_MULT
				#pragma shader_feature _ALPHA_CUTOFF

				// make fog work
				#pragma multi_compile_fog
				
				#include "Terrain.cginc"

				ENDCG
			}
		}
	}
	CustomEditor "TerrainShaderGUI"
}
