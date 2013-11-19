Shader "Custom/ScanShader"
{
  Properties 
  {
    _ScanColor ("Scan Tint", Color) = (1.0,1.7,1.0,1.0)
    _LineTex ("ScanLineTexture", 2D) = "white" {}
    _NoiseTex ("NoiseTexture", 2D) = "white" {}
    _TimeScale ("Time Scale", float) = -15.0
    _TexScale ("Texture Scale", float) = 15.0
    _BlendAlpha ("Transparency Value",float) = 0.6
  }
  Category {
       Lighting On
       ZWrite Off
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
       Tags {Queue=Transparent}
	  SubShader 
	  {
	   
	    Pass
	  	{
	  	Tags {"LightMode" = "ForwardBase"}
	      CGPROGRAM
	
	      //pragmas
	      #pragma vertex vert 
		  #pragma fragment frag
		  
	      // user defined variables
	      uniform float4 _ScanColor;
		  uniform int mask1=9397957;
		  uniform int mask2 = 9964396;
		  
		  uniform sampler2D _LineTex;  
		  uniform sampler2D _NoiseTex;  
		  uniform float _TimeScale;
		  uniform float _TexScale;
		  uniform float _BlendAlpha;
		  
		  uniform float4 _LightColor0;
		  
		  int modeVal=0;
		    // Base input Structures
	      struct vertexInput {
		      float4 vertex : POSITION;
		      float3 normal : NORMAL;
		      float4 texcoord : TEXCOORD0;
		    };
		    struct vertexOutput {
		      float4 pos : SV_POSITION;
		      float4 col : COLOR;
		      float4 tex : TEXCOORD0;
		    };
		  
	      // vertex shader
	      vertexOutput vert(vertexInput i)
	      {
	        vertexOutput o;
	        
	
	        o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
	        o.tex = i.texcoord;
	        return o;
	      }
	
		    // fragment shader
		    float4 frag(vertexOutput i) : COLOR
		    {
			
		    
		  
		     float4 texCol = tex2D(_LineTex,float2(i.tex.xy)*_TexScale+float2(0.0,_TimeScale*_Time));
			
		     
		     	
			 float4 modCol = tex2D(_NoiseTex,float2(i.tex.xy)*(-_Time));	     
		     return float4(0.0,texCol.x+modCol.x*0.1,0.0,_BlendAlpha);

	
		    }
		  
	      ENDCG
		  }
		  }
  }
}