Shader "Stephen/Toon/ToonX" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		
		_MainTex ("Albedo", 2D) = "white" {}

		[NoScaleOffset]	_MetallicTex ("Metallic Map (R)", 2D) = "black" {}
		_Metallic ("Metallic Scale", Range(0, 1)) = 0.0

		_Smoothness ("Smoothness", Range(0, 1)) = 0.1
		
		[NoScaleOffset]	_BumpTex ("Normal Map", 2D) = "bump" {}
		_BumpScale ("Normal Map Scale", Float) = 1.0

		[NoScaleOffset] _ParallaxTex ("Parallax", 2D) = "black" {}
		_ParallaxStrength ("Parallax Strength", Range(0, 0.1)) = 0

		[NoScaleOffset] _OcclusionTex ("Occlusion", 2D) = "white" {}
		_OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1

		[NoScaleOffset] _EmissionTex ("Emission Map", 2D) = "black" {}
		_Emission ("Emission", Color) = (0, 0, 0)

		_DetailTex ("Detail Albedo", 2D) = "gray" {}
		_DetailScale ("Detail Albedo Scale", Range (0.0, 1.0)) = 1
		[NoScaleOffset] _DetailBumpTex ("Detail Normals", 2D) = "bump" {}
		_DetailBumpScale ("Detail Bump Scale", Float) = 1

		[NoScaleOffset] _DetailMask ("Detail Mask", 2D) = "white" {}

		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 

		[NoScaleOffset]	_RimTex ("Rim Light", 2D) = "white" {}
		[NoScaleOffset]	_RimRampTex ("Rim Ramp", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range (0.125, 32.0)) = 1.0
		_RimScroll("Rim Scroll Speed", Vector) = (0,0,0,0)
		
		_ShinyRamp("Shiny Ramp (RGB)", 2D) = "black" {}
		_ShinyColor("Shiny Color", Color) = (1,1,1,1)
		_ShinySpeed("Shiny Speed", Float) = 8.0
		
		_DiscScale ("Discard Amount", Range (0.0, 1.0)) = 0.0
		_DiscSize ("Discard Size", Range (0.1, 100.0)) = 10.0

		_AlphaCutoff ("Alpha Cutoff", Range (0.0, 1.0)) = 0.5

		_DitherTex ("Dither Texture (R)", 2D) = "white" {}
		_DitherDistance ("Dither Distance", Float) = 1

		//_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		//_Outline ("Outline Width", Range (.002, 0.03)) = 0.01
		//_OutlineColor1 ("2nd Outline Color", Color) = (0,0,0,1)
		//_Outline1 ("2nd Outline Width", Range (.002, 0.03)) = 0.005

		[HideInInspector] _SrcBlend ("_SrcBlend", Int) = 1
		[HideInInspector] _DstBlend ("_DstBlend", Int) = 0
		[HideInInspector] _ZWrite ("_ZWrite", Int) = 1
		[HideInInspector] _Cull ("_Cull", Int) = 2
		[HideInInspector] _RenderQueueOffset ("Render Queue Offset", Int) = 0
		[HideInInspector] _Offset ("ZTest Offset", Vector) = (0,0,0,0)
	}

	CGINCLUDE

	#define BINORMAL_PER_FRAGMENT
	#define FOG_DISTANCE

	ENDCG

	SubShader 
	{
		Pass 
		{
			Tags 
			{
				"LightMode" = "ForwardBase"
			}
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			Cull [_Cull]
			Offset [_Offset.x], [_Offset.y]

			CGPROGRAM

			#pragma target 5.0

			#pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
			#pragma shader_feature _DITHER_MAP
			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _ _LIGHT_TOON
			#pragma shader_feature _TOON_COLOR
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _PARALLAX_MAP
			#pragma shader_feature _OCCLUSION_MAP
			#pragma shader_feature _EMISSION_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP
			#pragma shader_feature _RIM_LIGHT
			#pragma shader_feature _RIM_LIGHT_MAP
			#pragma shader_feature _RIM_LIGHT_RAMP
			#pragma shader_feature _RIM_SCROLL
			#pragma shader_feature _RIM_REVERSE	
			#pragma shader_feature _SHINY_RAMP
			#pragma shader_feature _SHINY_METALLIC_TEX
			#pragma shader_feature _DISCARD_INTERP

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma instancing_options lodfade force_same_maxcount_for_gl

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#define FORWARD_BASE_PASS

			
			

			#include "../Editor/Lighting.cginc"
			ENDCG
		}

		Pass 
		{
			Tags 
			{
				"LightMode" = "ForwardAdd"
			}

			Blend [_SrcBlend] One
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0

			#pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _PARALLAX_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP			
			#pragma shader_feature _RIM_LIGHT

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			
			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "../Editor/Lighting.cginc"


			ENDCG
		}

		Pass 
		{
			Tags 
			{
				"LightMode" = "Deferred"
			}

			ZWrite [_ZWrite]
			Cull [_Cull]
			Offset [_Offset.x], [_Offset.y]

			CGPROGRAM

			#pragma target 3.0
			#pragma exclude_renderers nomrt

			#pragma shader_feature _ _RENDERING_CUTOUT
			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _PARALLAX_MAP
			#pragma shader_feature _OCCLUSION_MAP
			#pragma shader_feature _EMISSION_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP
			#pragma shader_feature _RIM_LIGHT
			#pragma shader_feature _RIM_LIGHT_MAP
			#pragma shader_feature _RIM_LIGHT_RAMP
			#pragma shader_feature _RIM_SCROLL
			#pragma shader_feature _RIM_REVERSE	
			#pragma shader_feature _SHINY_RAMP
			#pragma shader_feature _SHINY_METALLIC_TEX
			#pragma shader_feature _DISCARD_INTERP

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_prepassfinal
			#pragma multi_compile_instancing
			#pragma instancing_options lodfade

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#define DEFERRED_PASS

			#include "../Editor/Lighting.cginc"

			ENDCG
		}

		Pass 
		{
			Tags 
			{
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
			#pragma shader_feature _SEMITRANSPARENT_SHADOWS
			#pragma shader_feature _SMOOTHNESS_ALBEDO
			#pragma shader_feature _DISCARD_INTERP

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing
			#pragma instancing_options lodfade force_same_maxcount_for_gl

			#pragma vertex MyShadowVertexProgram
			#pragma fragment MyShadowFragmentProgram

			#include "../Editor/Shadows.cginc"

			ENDCG
		}

		Pass 
		{
			Tags 
			{
				"LightMode" = "Meta"
			}

			Cull Off

			CGPROGRAM

			#pragma vertex MyLightmappingVertexProgram
			#pragma fragment MyLightmappingFragmentProgram

			#pragma shader_feature _METALLIC_MAP
			#pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
			#pragma shader_feature _EMISSION_MAP
			#pragma shader_feature _DETAIL_MASK
			#pragma shader_feature _DETAIL_ALBEDO_MAP

			#include "../Editor/Lightmapping.cginc"

			ENDCG
		}		
	}

	CustomEditor "ToonShaderGUI"
}