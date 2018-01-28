// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Stephen/Projector/Mult"
{
	Properties 
	{
	   _ShadowTex ("Projected Image", 2D) = "white" {}
	   _Amount ("Amount", Range(0,1)) = 1.0
	   _ProjScale ("Projection Scale", Vector) = (1,1,1,1) 
	}
	SubShader 
	{
    	Pass 
		{      
    		//Blend One One 
    		   // add color of _ShadowTex to the color in the framebuffer 
    		Blend DstColor Zero
			ZWrite Off // don't change depths
    		Offset -1, -1 // avoid depth fighting

    		CGPROGRAM
 
    		#pragma vertex vert  
    		#pragma fragment frag 
 
    		// User-specified properties
    		uniform sampler2D _ShadowTex; 
			uniform float _Amount;
			uniform float4 _ProjScale;
 
    		// Projector-specific uniforms
    		uniform float4x4 unity_Projector; // transformation matrix 
    		// from object space to projector space 
 
    		struct vertexInput 
			{
    			float4 vertex : POSITION;
    			float3 normal : NORMAL;
    		};

    		struct vertexOutput 
			{
    			float4 pos : SV_POSITION;
    			float4 posProj : TEXCOORD0;
    			// position in projector space
    		};
 
    		vertexOutput vert(vertexInput input) 
    		{
    			vertexOutput output;
 
    			output.posProj = mul(unity_Projector, input.vertex);
    			output.pos = UnityObjectToClipPos(input.vertex);
    			return output;
    		}
 
 
    		float4 frag(vertexOutput input) : COLOR
    		{
    			if (input.posProj.w > 0.0) // in front of projector?
    			{
					float2 offset = input.posProj.xy * _ProjScale.xy + _Time.r * _ProjScale.zw;
    				return (1-_Amount) + _Amount * tex2D(_ShadowTex , 
    				   offset / input.posProj.w); 
    				// alternatively: return tex2Dproj(  
    				//    _ShadowTex, input.posProj);
    			}
    			else // behind projector
    			{
    				return float4(0.0, 0.0, 0.0, 0.0);
    			}
    		}
 
    		ENDCG
    	}
   }  
   Fallback "Projector/Light"
}