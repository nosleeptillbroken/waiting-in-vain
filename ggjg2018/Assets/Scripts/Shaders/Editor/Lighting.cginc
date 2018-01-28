﻿// Upgrade NOTE: upgraded instancing buffer 'InstanceProperties' to new syntax.

#if !defined(SAT_LIGHTING_INCLUDED)
#define SAT_LIGHTING_INCLUDED

#include "SatMath.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	#if !defined(FOG_DISTANCE)
		#define FOG_DEPTH 1
	#endif
	#define FOG_ON 1
#endif

#if !defined(LIGHTMAP_ON) && defined(SHADOWS_SCREEN)
	#if defined(SHADOWS_SHADOWMASK) && !defined(UNITY_NO_SCREENSPACE_SHADOWS)
		#define ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS 1
	#endif
#endif

#if defined(LIGHTMAP_ON) && defined(SHADOWS_SCREEN)
	#if defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK)
		#define SUBTRACTIVE_LIGHTING 1
	#endif
#endif

UNITY_INSTANCING_BUFFER_START(InstanceProperties)
	UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr InstanceProperties
UNITY_INSTANCING_BUFFER_END(InstanceProperties)

sampler2D _MainTex, _DetailTex, _DetailMask;
float4 _MainTex_ST, _DetailTex_ST;
float _DetailScale;

sampler2D _BumpTex, _DetailBumpTex;
float _BumpScale, _DetailBumpScale;

sampler2D _MetallicTex;
float _Metallic;
float _Smoothness;

sampler2D _ParallaxTex;
float _ParallaxStrength;

sampler2D _OcclusionTex;
float _OcclusionStrength;

sampler2D _EmissionTex;
float3 _Emission;

sampler2D _Ramp;

sampler2D _RimTex;
sampler2D _RimRampTex;
float4 _RimColor;
float _RimPower;
float4 _RimScroll;

sampler2D _ShinyRamp;
float4 _ShinyColor;
float _ShinySpeed;

float _DiscScale;
float _DiscSize;

float _AlphaCutoff;

sampler2D _DitherTex;
float4 _DitherTex_TexelSize;
float _DitherDistance;

struct VertexData 
{
	UNITY_VERTEX_INPUT_INSTANCE_ID
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float4 color : COLOR;
	float2 uv : TEXCOORD0;
	float2 uv1 : TEXCOORD1;
	float2 uv2 : TEXCOORD2;
};

struct InterpolatorsVertex 
{
	UNITY_VERTEX_INPUT_INSTANCE_ID
	float4 pos : SV_POSITION;	
	float4 color : COLOR;
	float4 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;

	#if defined(BINORMAL_PER_FRAGMENT)
		float4 tangent : TEXCOORD2;
	#else
		float3 tangent : TEXCOORD2;
		float3 binormal : TEXCOORD3;
	#endif

	#if FOG_DEPTH
		float4 worldPos : TEXCOORD4;
	#else
		float3 worldPos : TEXCOORD4;
	#endif

	UNITY_SHADOW_COORDS(5)

	#if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD6;
	#endif

	#if defined(LIGHTMAP_ON) || ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
		float2 lightmapUV : TEXCOORD6;
	#endif

	#if defined(DYNAMICLIGHTMAP_ON)
		float2 dynamicLightmapUV : TEXCOORD7;
	#endif

	#if defined(_PARALLAX_MAP)
		float3 tangentViewDir : TEXCOORD8;
	#endif
    
	#if defined(_SHINY_RAMP) || defined(_DITHER_MAP)
		float4 screenPos : TEXCOORD9;
	#endif
};

struct Interpolators 
{
	UNITY_VERTEX_INPUT_INSTANCE_ID
	#if defined(LOD_FADE_CROSSFADE) || defined(_DITHER_MAP)
		UNITY_VPOS_TYPE vpos : VPOS;
	#else
		float4 pos : SV_POSITION;
	#endif

	float4 color : COLOR;
	float4 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;

	#if defined(BINORMAL_PER_FRAGMENT)
		float4 tangent : TEXCOORD2;
	#else
		float3 tangent : TEXCOORD2;
		float3 binormal : TEXCOORD3;
	#endif

	#if FOG_DEPTH
		float4 worldPos : TEXCOORD4;
	#else
		float3 worldPos : TEXCOORD4;
	#endif

	UNITY_SHADOW_COORDS(5)

	#if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD6;
	#endif

	#if defined(LIGHTMAP_ON) || ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
		float2 lightmapUV : TEXCOORD6;
	#endif

	#if defined(DYNAMICLIGHTMAP_ON)
		float2 dynamicLightmapUV : TEXCOORD7;
	#endif

	#if defined(_PARALLAX_MAP)
		float3 tangentViewDir : TEXCOORD8;
	#endif

	#if defined(_SHINY_RAMP) || defined(_DITHER_MAP)
		float4 screenPos : TEXCOORD9;
	#endif
};

float GetDetailMask (Interpolators i) 
{
	#if defined (_DETAIL_MASK)
		return tex2D(_DetailMask, i.uv.xy).a;
	#else
		return 1;
	#endif
}

float3 GetAlbedo (Interpolators i) 
{
	float3 albedo =
		tex2D(_MainTex, i.uv.xy).rgb * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).rgb;
	#if defined (_DETAIL_ALBEDO_MAP)
		float3 details = tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;
		
		albedo = lerp(albedo, albedo * details, _DetailScale * GetDetailMask(i));
	#endif
	return albedo;
}

float GetAlpha (Interpolators i) 
{
	float alpha = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).a;
	#if !defined(_SMOOTHNESS_ALBEDO)
		alpha *= tex2D(_MainTex, i.uv.xy).a;
	#endif
	return alpha;
}

float3 GetTangentSpaceNormal (Interpolators i) 
{
	float3 normal = float3(0, 0, 1);
	#if defined(_NORMAL_MAP)
		normal = UnpackScaleNormal(tex2D(_BumpTex, i.uv.xy), _BumpScale);
	#endif
	#if defined(_DETAIL_NORMAL_MAP)
		float3 detailNormal =
			UnpackScaleNormal(
				tex2D(_DetailBumpTex, i.uv.zw), _DetailBumpScale);
		detailNormal = lerp(float3(0, 0, 1), detailNormal, GetDetailMask(i));
		normal = BlendNormals(normal, detailNormal);
	#endif
	return normal;
}

float GetMetallic (Interpolators i) 
{
	#if defined(_METALLIC_MAP)
		return tex2D(_MetallicTex, i.uv.xy).r;
	#else
		return _Metallic;
	#endif
}

float GetShinyMetallic (Interpolators i)
{
	#if defined(_SHINY_METALLIC_TEX) && defined(_METALLIC_MAP)
		return tex2D(_MetallicTex, i.uv.xy).g;
	#else
		return 1;
	#endif
}

float GetSmoothness (Interpolators i) 
{
	float smoothness = 1;
	#if defined(_SMOOTHNESS_ALBEDO)
		smoothness = tex2D(_MainTex, i.uv.xy).a;
	#elif defined(_SMOOTHNESS_METALLIC) && defined(_METALLIC_MAP)
		smoothness = tex2D(_MetallicTex, i.uv.xy).a;
	#endif
	return smoothness * _Smoothness;
}

float GetOcclusion (Interpolators i) 
{
	#if defined(_OCCLUSION_MAP)
		return lerp(1, tex2D(_OcclusionTex, i.uv.xy).g, _OcclusionStrength);
	#else
		return 1;
	#endif
}

float3 GetEmission (Interpolators i) 
{
	#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
		#if defined(_EMISSION_MAP)
			#if	defined(_RIM_SCROLL)	 
				return tex2D(_EmissionTex, i.uv.xy + _Time.gg * _RimScroll.zw) * _Emission;
			#else
				return tex2D(_EmissionTex, i.uv.xy) * _Emission;
			#endif
		#else
			return _Emission;
		#endif
	#else
		return 0;
	#endif
}

float3 GetShiny(Interpolators i)
{
	#if defined(_SHINY_RAMP)	
		float2 screenUV = i.screenPos.xy / i.screenPos.w;
		float offset = tex2D(_ShinyRamp, float2(screenUV.x, screenUV.y)).w;
		screenUV = (-_Time.r * _ShinySpeed + offset * 0.04 + (screenUV.y + screenUV.x * 0.75)*0.3).xx;
		return _ShinyColor * tex2D(_ShinyRamp, screenUV).rgb * GetShinyMetallic(i);
	#else
		return 0;
	#endif
}

float3 GetRim (Interpolators i, float3 viewDir) 
{
	#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
		#if defined(_RIM_LIGHT)
			#if defined(_RIM_REVERSE)
				half rim = saturate(dot (viewDir, i.normal));
			#else
				half rim = 1.0 - (dot (viewDir, i.normal));
			#endif
			rim = pow (rim, _RimPower);

			#if defined(_RIM_SCROLL)
				#if defined(_RIM_LIGHT_RAMP)			
					#if defined(_RIM_LIGHT_MAP)
						half4 rimCol = tex2D(_RimTex, i.uv.xy + _Time.gg * _RimScroll.xy);
						return rim.x * _RimColor.rgb * tex2D(_RimRampTex, rim).rgb * rimCol.rgb * rimCol.a;
					#else
						return rim.x * _RimColor.rgb * tex2D(_RimRampTex, rim).rgb * rim;
					#endif
				#else
					#if defined(_RIM_LIGHT_MAP)
						half4 rimCol = tex2D(_RimTex, i.uv.xy + _Time.gg * _RimScroll.xy);
						return rim.x * _RimColor.rgb * rim * rimCol.rgb * rimCol.a;
					#else
						return rim.x * _RimColor.rgb * rim;
					#endif
				#endif
			#else
				#if defined(_RIM_LIGHT_RAMP)			
					#if defined(_RIM_LIGHT_MAP)
						half4 rimCol = tex2D(_RimTex, i.uv.xy);
						return rim.x * _RimColor.rgb * tex2D(_RimRampTex, rim).rgb * rimCol.rgb * rimCol.a;
					#else
						return rim.x * _RimColor.rgb * tex2D(_RimRampTex, rim).rgb * rim;
					#endif
				#else
					#if defined(_RIM_LIGHT_MAP)
						half4 rimCol = tex2D(_RimTex, i.uv.xy);
						return rim.x * _RimColor.rgb * rim * rimCol.rgb * rimCol.a;
					#else
						return rim.x * _RimColor.rgb * rim;
					#endif
				#endif
			#endif
		#endif
	#endif
	return 0;
}

void ComputeVertexLightColor (inout Interpolators i) 
{
	#if defined(VERTEXLIGHT_ON)
		i.vertexLightColor = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb,
			unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, i.worldPos.xyz, i.normal);
	#endif
}

float3 CreateBinormal (float3 normal, float3 tangent, float binormalSign) 
{
	return cross(normal, tangent.xyz) *
		(binormalSign * unity_WorldTransformParams.w);
}

InterpolatorsVertex MyVertexProgram (VertexData v) 
{
	InterpolatorsVertex i;
	UNITY_INITIALIZE_OUTPUT(InterpolatorsVertex, i);
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, i);
	i.pos = UnityObjectToClipPos(v.vertex);
	i.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex);
	i.color = v.color;
	#if FOG_DEPTH
		i.worldPos.w = i.pos.z;
	#endif
	i.normal = UnityObjectToWorldNormal(v.normal);

	#if defined(BINORMAL_PER_FRAGMENT)
		i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
	#else
		i.tangent = UnityObjectToWorldDir(v.tangent.xyz);
		i.binormal = CreateBinormal(i.normal, i.tangent, v.tangent.w);
	#endif

	i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
	i.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex);

	#if defined(LIGHTMAP_ON) || ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
		i.lightmapUV = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif

	#if defined(DYNAMICLIGHTMAP_ON)
		i.dynamicLightmapUV =
			v.uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	#endif

	UNITY_TRANSFER_SHADOW(i, v.uv1);

	ComputeVertexLightColor(i);

	#if defined (_PARALLAX_MAP)
		#if defined(PARALLAX_SUPPORT_SCALED_DYNAMIC_BATCHING)
			v.tangent.xyz = normalize(v.tangent.xyz);
			v.normal = normalize(v.normal);
		#endif
		float3x3 objectToTangent = float3x3(
			v.tangent.xyz,
			cross(v.normal, v.tangent.xyz) * v.tangent.w,
			v.normal);

		i.tangentViewDir = mul(objectToTangent, ObjSpaceViewDir(v.vertex));
	#endif

	#if defined(_SHINY_RAMP) || defined(_DITHER_MAP)
  		i.screenPos = ComputeScreenPos(i.pos);
	#endif

	return i;
}

float FadeShadows (Interpolators i, float attenuation) 
{
	#if HANDLE_SHADOWS_BLENDING_IN_GI || ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
		// UNITY_LIGHT_ATTENUATION doesn't fade shadows for us.
		#if ADDITIONAL_MASKED_DIRECTIONAL_SHADOWS
			attenuation = SHADOW_ATTENUATION(i);
		#endif
		float viewZ =
			dot(_WorldSpaceCameraPos - i.worldPos, UNITY_MATRIX_V[2].xyz);
		float shadowFadeDistance =
			UnityComputeShadowFadeDistance(i.worldPos, viewZ);
		float shadowFade = UnityComputeShadowFade(shadowFadeDistance);
		float bakedAttenuation =
			UnitySampleBakedOcclusion(i.lightmapUV, i.worldPos);
		attenuation = UnityMixRealtimeAndBakedShadows(
			attenuation, bakedAttenuation, shadowFade);
	#endif

	return attenuation;
}

UnityLight CreateLight (Interpolators i) 
{
	UnityLight light;

	#if defined(DEFERRED_PASS) || SUBTRACTIVE_LIGHTING
		light.dir = float3(0, 1, 0);
		light.color = 0;
	#else
		#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
			light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
		#else
			light.dir = _WorldSpaceLightPos0.xyz;
		#endif

		UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
		attenuation = FadeShadows(i, attenuation);

		light.color = _LightColor0.rgb * attenuation;
	#endif

	return light;
}

float3 BoxProjection (
	float3 direction, float3 position,
	float4 cubemapPosition, float3 boxMin, float3 boxMax
) 
{
	#if UNITY_SPECCUBE_BOX_PROJECTION
		UNITY_BRANCH
		if (cubemapPosition.w > 0) 
		{
			float3 factors =
				((direction > 0 ? boxMax : boxMin) - position) / direction;
			float scalar = min(min(factors.x, factors.y), factors.z);
			direction = direction * scalar + (position - cubemapPosition);
		}
	#endif
	return direction;
}

void ApplySubtractiveLighting (
	Interpolators i, inout UnityIndirect indirectLight
) 
{
	#if SUBTRACTIVE_LIGHTING
		UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
		attenuation = FadeShadows(i, attenuation);

		float ndotl = saturate(dot(i.normal, _WorldSpaceLightPos0.xyz));
		float3 shadowedLightEstimate =
			ndotl * (1 - attenuation) * _LightColor0.rgb;
		float3 subtractedLight = indirectLight.diffuse - shadowedLightEstimate;
		subtractedLight = max(subtractedLight, unity_ShadowColor.rgb);
		subtractedLight =
			lerp(subtractedLight, indirectLight.diffuse, _LightShadowData.x);
		indirectLight.diffuse = min(subtractedLight, indirectLight.diffuse);
	#endif
}

UnityIndirect CreateIndirectLight (Interpolators i, float3 viewDir) 
{
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

	#if defined(VERTEXLIGHT_ON)
		indirectLight.diffuse = i.vertexLightColor;
	#endif

	#if defined(FORWARD_BASE_PASS) || defined(DEFERRED_PASS)
		#if defined(LIGHTMAP_ON)
			indirectLight.diffuse =
				DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightmapUV));
			
			#if defined(DIRLIGHTMAP_COMBINED)
				float4 lightmapDirection = UNITY_SAMPLE_TEX2D_SAMPLER(
					unity_LightmapInd, unity_Lightmap, i.lightmapUV);
				indirectLight.diffuse = DecodeDirectionalLightmap(
					indirectLight.diffuse, lightmapDirection, i.normal);
			#endif

			ApplySubtractiveLighting(i, indirectLight);
		#endif

		#if defined(DYNAMICLIGHTMAP_ON)
			float3 dynamicLightDiffuse = DecodeRealtimeLightmap(
				UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, i.dynamicLightmapUV));

			#if defined(DIRLIGHTMAP_COMBINED)
				float4 dynamicLightmapDirection = UNITY_SAMPLE_TEX2D_SAMPLER(
					unity_DynamicDirectionality, unity_DynamicLightmap,
					i.dynamicLightmapUV);
            	indirectLight.diffuse += DecodeDirectionalLightmap(
            		dynamicLightDiffuse, dynamicLightmapDirection, i.normal);
			#else
				indirectLight.diffuse += dynamicLightDiffuse;
			#endif
		#endif

		#if !defined(LIGHTMAP_ON) && !defined(DYNAMICLIGHTMAP_ON)
			#if UNITY_LIGHT_PROBE_PROXY_VOLUME
				if (unity_ProbeVolumeParams.x == 1)
				{
					indirectLight.diffuse = SHEvalLinearL0L1_SampleProbeVolume(
						float4(i.normal, 1), i.worldPos);
					indirectLight.diffuse = max(0, indirectLight.diffuse);
					#if defined(UNITY_COLORSPACE_GAMMA)
			            indirectLight.diffuse =
			            	LinearToGammaSpace(indirectLight.diffuse);
			        #endif
				}
				else 
				{
					indirectLight.diffuse +=
						max(0, ShadeSH9(float4(i.normal, 1)));
				}
			#else
				indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
			#endif
		#endif

		float3 reflectionDir = reflect(-viewDir, i.normal);
		Unity_GlossyEnvironmentData envData;
		envData.roughness = 1 - GetSmoothness(i);
		envData.reflUVW = BoxProjection(
			reflectionDir, i.worldPos.xyz,
			unity_SpecCube0_ProbePosition,
			unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
		float3 probe0 = Unity_GlossyEnvironment(
			UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData);
		envData.reflUVW = BoxProjection(
			reflectionDir, i.worldPos.xyz,
			unity_SpecCube1_ProbePosition,
			unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax);
		#if UNITY_SPECCUBE_BLENDING
			float interpolator = unity_SpecCube0_BoxMin.w;
			UNITY_BRANCH
			if (interpolator < 0.999999) 
			{
				float3 probe1 = Unity_GlossyEnvironment(
					UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0),
					unity_SpecCube0_HDR, envData);
				indirectLight.specular = lerp(probe1, probe0, interpolator);
			}
			else 
			{
				indirectLight.specular = probe0;
			}
		#else
			indirectLight.specular = probe0;
		#endif

		float occlusion = GetOcclusion(i);
		indirectLight.diffuse *= occlusion;
		indirectLight.specular *= occlusion;

		#if defined(DEFERRED_PASS) && UNITY_ENABLE_REFLECTION_BUFFERS
			indirectLight.specular = 0;
		#endif
	#endif

	return indirectLight;
}

void InitializeFragmentNormal(inout Interpolators i) 
{
	float3 tangentSpaceNormal = GetTangentSpaceNormal(i);
	#if defined(BINORMAL_PER_FRAGMENT)
		float3 binormal = CreateBinormal(i.normal, i.tangent.xyz, i.tangent.w);
	#else
		float3 binormal = i.binormal;
	#endif
	
	i.normal = normalize(
		tangentSpaceNormal.x * i.tangent +
		tangentSpaceNormal.y * binormal +
		tangentSpaceNormal.z * i.normal);
}

float4 ApplyFog (float4 color, Interpolators i) 
{
	#if FOG_ON
		float viewDistance = length(_WorldSpaceCameraPos - i.worldPos.xyz);
		#if FOG_DEPTH
			viewDistance = UNITY_Z_0_FAR_FROM_CLIPSPACE(i.worldPos.w);
		#endif
		UNITY_CALC_FOG_FACTOR_RAW(viewDistance);
		float3 fogColor = 0;
		#if defined(FORWARD_BASE_PASS)
			fogColor = unity_FogColor.rgb;
		#endif
		color.rgb = lerp(fogColor, color.rgb, saturate(unityFogFactor));
	#endif
	return color;
}

float GetParallaxHeight (float2 uv) 
{
	return tex2D(_ParallaxTex, uv).g;
}

float2 ParallaxOffset (float2 uv, float2 viewDir) 
{
	float height = GetParallaxHeight(uv);
	height -= 0.5;
	height *= _ParallaxStrength;
	return viewDir * height;
}

float2 ParallaxRaymarching (float2 uv, float2 viewDir) 
{
	#if !defined(PARALLAX_RAYMARCHING_STEPS)
		#define PARALLAX_RAYMARCHING_STEPS 10
	#endif
	float2 uvOffset = 0;
	float stepSize = 1.0 / PARALLAX_RAYMARCHING_STEPS;
	float2 uvDelta = viewDir * (stepSize * _ParallaxStrength);

	float stepHeight = 1;
	float surfaceHeight = GetParallaxHeight(uv);

	float2 prevUVOffset = uvOffset;
	float prevStepHeight = stepHeight;
	float prevSurfaceHeight = surfaceHeight;

	for (int i = 1;
		i < PARALLAX_RAYMARCHING_STEPS && stepHeight > surfaceHeight;
		i++) 
	{
		prevUVOffset = uvOffset;
		prevStepHeight = stepHeight;
		prevSurfaceHeight = surfaceHeight;
		
		uvOffset -= uvDelta;
		stepHeight -= stepSize;
		surfaceHeight = GetParallaxHeight(uv + uvOffset);
	}

	#if !defined(PARALLAX_RAYMARCHING_SEARCH_STEPS)
		#define PARALLAX_RAYMARCHING_SEARCH_STEPS 0
	#endif
	#if PARALLAX_RAYMARCHING_SEARCH_STEPS > 0
		for (int i = 0; i < PARALLAX_RAYMARCHING_SEARCH_STEPS; i++) 
		{
			uvDelta *= 0.5;
			stepSize *= 0.5;

			if (stepHeight < surfaceHeight) 
			{
				uvOffset += uvDelta;
				stepHeight += stepSize;
			}
			else 
			{
				uvOffset -= uvDelta;
				stepHeight -= stepSize;
			}
			surfaceHeight = GetParallaxHeight(uv + uvOffset);
		}
	#elif defined(PARALLAX_RAYMARCHING_INTERPOLATE)
		float prevDifference = prevStepHeight - prevSurfaceHeight;
		float difference = surfaceHeight - stepHeight;
		float t = prevDifference / (prevDifference + difference);
		uvOffset = prevUVOffset - uvDelta * t;
	#endif

	return uvOffset;
}

void ApplyParallax (inout Interpolators i) 
{
	#if defined(_PARALLAX_MAP)
		i.tangentViewDir = normalize(i.tangentViewDir);
		#if !defined(PARALLAX_OFFSET_LIMITING)
			#if !defined(PARALLAX_BIAS)
				#define PARALLAX_BIAS 0.42
			#endif
			i.tangentViewDir.xy /= (i.tangentViewDir.z + PARALLAX_BIAS);
		#endif

		#if !defined(PARALLAX_FUNCTION)
			#define PARALLAX_FUNCTION ParallaxOffset
		#endif
		float2 uvOffset = PARALLAX_FUNCTION(i.uv.xy, i.tangentViewDir.xy);
		i.uv.xy += uvOffset;
		i.uv.zw += uvOffset * (_DetailTex_ST.xy / _MainTex_ST.xy);
	#endif
}

half4 ApplyToon(
	half3 diffColor, half3 specColor, 
	half oneMinusReflectivity, half smoothness, 
	half3 normal, half3 viewDir, 
	UnityLight light, UnityIndirect gi)
{
	half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);
	
	#if defined(_TOON_COLOR)
		half3 nl = tex2D(_Ramp, dot(normal, light.dir) * 0.5 + 0.5).rgb;
	#else
		half nl = tex2D(_Ramp, dot(normal, light.dir) * 0.5 + 0.5).r;
	#endif
	half nh = saturate(dot(normal, halfDir));
    half nv = saturate(dot(normal, viewDir));
    half lh = saturate(dot(light.dir, halfDir));

    // Specular term
    half perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
    half roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

#if UNITY_BRDF_GGX

    // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
    // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
    // https://community.arm.com/events/1155
    half a = roughness;
    half a2 = a*a;

    half d = nh * nh * (a2 - 1.h) + 1.00001h;
#ifdef UNITY_COLORSPACE_GAMMA
    // Tighter approximation for Gamma only rendering mode!
    // DVF = sqrt(DVF);
    // DVF = (a * sqrt(.25)) / (max(sqrt(0.1), lh)*sqrt(roughness + .5) * d);
    half specularTerm = a / (max(0.32h, lh) * (1.5h + roughness) * d);
#else
    half specularTerm = a2 / (max(0.1h, lh*lh) * (roughness + 0.5h) * (d * d) * 4);
#endif

    // on mobiles (where half actually means something) denominator have risk of overflow
    // clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
    // sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if defined (SHADER_API_MOBILE)
    specularTerm = specularTerm - 1e-4h;
#endif

#else

    // Legacy
    half specularPower = PerceptualRoughnessToSpecPower(perceptualRoughness);
    // Modified with approximate Visibility function that takes roughness into account
    // Original ((n+1)*N.H^n) / (8*Pi * L.H^3) didn't take into account roughness
    // and produced extremely bright specular at grazing angles

    half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness; // approx ModifiedKelemenVisibilityTerm(lh, perceptualRoughness);
    half invF = lh;

    half specularTerm = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);

#ifdef UNITY_COLORSPACE_GAMMA
    specularTerm = sqrt(max(1e-4h, specularTerm));
#endif

#endif

#if defined (SHADER_API_MOBILE)
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
    specularTerm = 0.0;
#endif

    // surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)

    // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
    // 1-x^3*(0.6-0.08*x)   approximation for 1/(x^4+1)
#ifdef UNITY_COLORSPACE_GAMMA
    half surfaceReduction = 0.28;
#else
    half surfaceReduction = (0.6-0.08*perceptualRoughness);
#endif

    surfaceReduction = 1.0 - roughness*perceptualRoughness*surfaceReduction;

    half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));
    half3 color =   (diffColor + specularTerm * specColor) * light.color * nl
                    + gi.diffuse * diffColor
                    + surfaceReduction * gi.specular * FresnelLerpFast (specColor, grazingTerm, nv);

    return half4(color, 1);
}

struct FragOut
{
	#if defined(DEFERRED_PASS)
		float4 gBuffer0 : SV_Target0;
		float4 gBuffer1 : SV_Target1;
		float4 gBuffer2 : SV_Target2;
		float4 gBuffer3 : SV_Target3;

		#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
			float4 gBuffer4 : SV_Target4;
		#endif
	#else
		float4 color : SV_Target;
	#endif
};

FragOut MyFragmentProgram (Interpolators i) 
{
	UNITY_SETUP_INSTANCE_ID(i);
	#if defined(LOD_FADE_CROSSFADE)
		UnityApplyDitherCrossFade(i.vpos);
	#endif

	#if defined(_DISCARD_INTERP)
		float2 pos = floor((i.uv.xy) * _DiscSize) / _DiscSize;
		float dis = rand(pos);
		dis += rand(i.worldPos.xy) * 0.1;
		if(dis < _DiscScale * (1 + 0.1))
			discard;
	#endif

	ApplyParallax(i);

	float alpha = GetAlpha(i);
	#if defined(_DITHER_MAP)
		//alpha -= saturate((fDist - _DitherDistance.y) / _DitherDistance.x);
		
		half2 screenPos = i.screenPos.xy / i.screenPos.w;
		half2 aspectRatio = half2(_ScreenParams.x/_ScreenParams.y,1.0f);
		alpha -= (1-i.vpos.w/_DitherDistance) * tex2D(_DitherTex, i.vpos.xy / _ScreenParams.xy * _DitherTex_TexelSize.zw * aspectRatio).r;
		//clip(alpha - 0.5f);
		//clip(tex2D(_DitherTex, screenPos).r - 0.03f);
		#if !defined(_RENDERING_CUTOUT)	
			clip(alpha - 0.95);
		#endif
	#endif
	
	

	#if defined(_RENDERING_CUTOUT)		
		clip(alpha - _AlphaCutoff);
	#endif

	InitializeFragmentNormal(i);

	float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);

	float3 specularTint;
	float oneMinusReflectivity;
	float3 albedo = DiffuseAndSpecularFromMetallic(
		GetAlbedo(i), GetMetallic(i), specularTint, oneMinusReflectivity);
	#if defined(_RENDERING_TRANSPARENT)
		albedo *= alpha;
		alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
	#endif
	float4 color = float(1).rrrr;
	
	#if defined(_LIGHT_TOON)
		color =  ApplyToon(
		albedo, specularTint,
		oneMinusReflectivity, GetSmoothness(i),
		i.normal, viewDir,
		CreateLight(i), CreateIndirectLight(i, viewDir));
	#else
	// defined(_LIGHT_PBR)
	// _LIGHT_PBR
		color = UNITY_BRDF_PBS(
		albedo, specularTint,
		oneMinusReflectivity, GetSmoothness(i),
		i.normal, viewDir,
		CreateLight(i), CreateIndirectLight(i, viewDir));
	#endif

	#if defined(_RENDERING_FADE) || defined(_RENDERING_TRANSPARENT)
		color.a = alpha;
	#endif

	color *= i.color;

	color.rgb += GetEmission(i);
	color.rgb += GetRim(i, viewDir);
	color.rgb += GetShiny(i);

	

	FragOut output;
	#if defined(DEFERRED_PASS)
		#if !defined(UNITY_HDR_ON)
			color.rgb = exp2(-color.rgb);
		#endif
		output.gBuffer0.rgb = albedo;
		output.gBuffer0.a = GetOcclusion(i);
		output.gBuffer1.rgb = specularTint;
		output.gBuffer1.a = GetSmoothness(i);
		output.gBuffer2 = float4(i.normal * 0.5 + 0.5, 1);
		output.gBuffer3 = color;

		#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
			float2 shadowUV = 0;
			#if defined(LIGHTMAP_ON)
				shadowUV = i.lightmapUV;
			#endif
			output.gBuffer4 =
				UnityGetRawBakedOcclusions(shadowUV, i.worldPos.xyz);
		#endif
	#else
		output.color = ApplyFog(color, i);
	#endif

	#if defined(_DITHER_MAP)
	//	half2 screensPos = i.screenPos.xy / i.screenPos.w;
	//	output.color.rgb = tex2D(_DitherTex, screensPos * _DitherTex_TexelSize.zw).rrr;
	//	output.color.rgb = tex2D(_DitherTex, i.vpos.zw).rrr;// * _DitherTex_TexelSize.zw * aspectRatio);
	//	output.color.b = 0.0f;
	//	output.color.rg = i.vpos.xy / _ScreenParams.xy;
	#endif

	return output;
}

#endif