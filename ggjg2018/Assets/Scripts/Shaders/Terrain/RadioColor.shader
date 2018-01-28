Shader "Stephen/Terrain/RadioSignal"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainTexColor ("Texture Color", Color) = (0.5, 0.5, 0.5, 0.5)
			
		_Cloud ("Cloud", 2D) = "white" {}	
		_SonarRamp ("Sonar Ramp", 2D) = "white" {}
		_SonarRampColor ("Sonar Color", Color) = (0.5, 0.5, 0.5, 0.5)
		uOffsetUVSize ("World Pos Offset", Vector) = (0.2, 0.2, 1.0, 1.0)
	}
	Category
	{
		Tags { "Queue"="Transparent" "RenderType"="Opaque" "DisableBatching" = "true"}

		SubShader
		{
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend SrcAlpha One
			ZWrite Off
			//LOD 100
			Offset -30, 0

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog
				
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uvMain : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					
					float4 vertex : SV_POSITION;
					float3 localPos : TEXCOORD2;
					float3 worldPos : TEXCOORD3;
				};

				sampler2D _MainTex;
				sampler2D _CameraDepthTexture;
				float4 _MainTexColor;
				float4 _MainTex_ST;
				
				sampler2D _Cloud;
				sampler2D _SonarRamp;

				float4 _SonarRampColor;
				float2 uOffsetUVSize;
				
				v2f vert (appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.localPos = v.vertex;

					o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
					    
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					half4 col = tex2D(_MainTex, i.uvMain) * _MainTexColor * unity_ColorSpaceDouble;
					//half4 col = 0;
					clip(col.a-0.01);
	
					float4 offset = 0.0;
					offset.x = tex2D(_Cloud, i.worldPos.xz * uOffsetUVSize.xy + _Time.x * 1.0).r;
					offset.w = offset.x + offset.x - 1;
					
			
					float fragDistance = length(i.localPos.xyz);
					float sonarValue = tex2D(_SonarRamp, fragDistance * 0.25 - _Time.r * 20.0f + offset * 0.05).r;
					float amount = saturate((fragDistance - (offset.x - offset.y) * 0.5));
					sonarValue = sonarValue / (fragDistance * 2.0 + 0.01);
					
					col.rgb += sonarValue * _SonarRampColor * unity_ColorSpaceDouble * col.a;
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
	}
}
