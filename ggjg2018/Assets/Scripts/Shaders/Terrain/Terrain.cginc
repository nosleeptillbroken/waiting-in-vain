#if !defined(SAT_TERRAIN_INCLUDED)
#define SAT_TERRAIN_INCLUDED

#include "UnityCG.cginc"
struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
 	//float3 normal : NORMAL;
   	//float4 tangent : TANGENT;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
	float2 uvMain : TEXCOORD0;
	UNITY_FOG_COORDS(1)
	
	float4 vertex : SV_POSITION;
	//float3 localPos : TEXCOORD3;
	#if defined(_SHINY_RAMP_WORLD_POS) || defined(_ALPHA_CUTOFF)
		float3 worldPos : TEXCOORD4;
	#endif

	#if defined(_SHINY_RAMP) || defined(_DITHER_MAP)
		float4 screenPos : TEXCOORD9;
	#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

sampler2D _MainTex;
float4 _MainTex_ST;

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4, _ColorMult)
UNITY_DEFINE_INSTANCED_PROP(float, _CloudCutoff)
UNITY_INSTANCING_BUFFER_END(Props)

sampler2D _CloudTex;

sampler2D _ShinyRamp;
float4 _ShinyColor;
float _ShinySpeed;
float _ShinyPosSize;

v2f vertTerrain (appdata v)
{
	v2f o;

	UNITY_SETUP_INSTANCE_ID(v);
   	UNITY_TRANSFER_INSTANCE_ID(v, o);

	o.vertex = UnityObjectToClipPos(v.vertex);
	#if defined(_SHINY_RAMP_WORLD_POS) || defined(_ALPHA_CUTOFF)
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
	#endif
	//o.localPos = v.vertex;
	o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
	    
	#if defined(_SHINY_RAMP) || defined(_DITHER_MAP)
  		o.screenPos = ComputeScreenPos(o.vertex);
	#endif
	UNITY_TRANSFER_FOG(o,o.vertex);
	return o;
}

float4 fragTerrain (v2f i) : SV_Target
{
	float4 ColorMult = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorMult) * unity_ColorSpaceDouble;
	float4 col = tex2D(_MainTex, i.uvMain) * ColorMult;
	
	#if defined(_ALPHA_CUTOFF)
		clip(tex2D(_CloudTex, i.worldPos.xz + _Time.r * 5.0).r - (1 - UNITY_ACCESS_INSTANCED_PROP(Props, _CloudCutoff)));
	#endif

	#if defined(_SHINY_RAMP)	
	#if !defined(_SHINY_RAMP_WORLD_POS)
		float2 screenUV = i.screenPos.xy / i.screenPos.w;
		float offset = tex2D(_ShinyRamp, float2(screenUV.x, screenUV.y)).w;
			screenUV = (-_Time.r * _ShinySpeed + offset * 0.04 + (screenUV.y + screenUV.x * 0.75)*0.3).xx;
		#if defined(_SHINY_RAMP_OFFSET)
			screenUV += _ColorMult.r * 4.4 + _ColorMult.g + _ColorMult.b * 5.1;
		#endif
		#if defined(_SHINY_RAMP_COLOR_MULT)
			col.rgb += _ShinyColor * tex2D(_ShinyRamp, screenUV).rgb * ColorMult;
		#else
			col.rgb += _ShinyColor * tex2D(_ShinyRamp, screenUV).rgb;
		#endif
	#else
		float offset = tex2D(_ShinyRamp, i.worldPos.xz).w;
			i.worldPos.xz = (-_Time.r * _ShinySpeed + offset * 0.04 + (i.worldPos.x + i.worldPos.z) * _ShinyPosSize).xx;
		#if defined(_SHINY_RAMP_OFFSET)
			i.worldPos.xz += ColorMult.r * 4.4 + ColorMult.g + ColorMult.b * 5.1;
		#endif
		#if defined(_SHINY_RAMP_COLOR_MULT)
			col.rgb += _ShinyColor * tex2D(_ShinyRamp, i.worldPos.xz).rgb * ColorMult;
		#else
			col.rgb += _ShinyColor * tex2D(_ShinyRamp, i.worldPos.xz).rgb;
		#endif
	#endif
	#endif

	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}
#endif