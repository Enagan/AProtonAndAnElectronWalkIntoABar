// Shader created with Shader Forge Beta 0.32 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.32;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32068,y:32626|emission-2-RGB,alpha-2-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:32413,y:32665,ptlb:node_2,ptin:_node_2,tex:f9cf3278d64cbdd47bd0b70bfffd3be5,ntxv:0,isnm:False|UVIN-10-OUT;n:type:ShaderForge.SFN_TexCoord,id:7,x:33195,y:32452,uv:0;n:type:ShaderForge.SFN_Code,id:10,x:32579,y:32468,code:ZgBsAG8AYQB0ACAAdQAgAD0AIAB4ACAAKwB0AGkAbQBlADsACgBmAGwAbwBhAHQAIAB2ACAAPQAgAHkAOwAKAAoAaQBmACgAeAAgAD4AIAAwAC4ANQApAAoAIAB1ACAAPQAgAHgALQB0AGkAbQBlADsACgAKAGYAbABvAGEAdAAgAGgAYQBsAGYAVgAgAD0AIAAxAC4AMAAvADMALgAwADsACgAKAAoAaQBmACgAeAAgADwAIAAwAC4ANQApAAoAewAKAAkAaQBmACgAeQAgADwAIABoAGEAbABmAFYAKQAKACAAIAB7AAoACQAgACAAdQAgAD0AIAB4ACsAeQAtAGgAYQBsAGYAVgAgACsAIAB0AGkAbQBlADsACgAgACAAfQAKACAAIABlAGwAcwBlACAAaQBmACAAKAB5ACAAPgAgADIALgAwACoAaABhAGwAZgBWACkACgAJAHsACgAJACAAdQAgAD0AIAB4AC0AeQAtAGgAYQBsAGYAVgAgACsAIAB0AGkAbQBlADsACgAJAH0ACgB9AAoAZQBsAHMAZQAKAHsACgAgACAAaQBmACgAeQAgADwAIABoAGEAbABmAFYAKQAKACAAIAB7AAoAIAAgACAAIAB1ACAAPQAgAC0AeAArAHkALQBoAGEAbABmAFYAIAArACAAdABpAG0AZQA7AAoAIAAgAH0AIAAKACAAIABlAGwAcwBlACAAaQBmACAAKAB5ACAAPgAgADIALgAwACoAaABhAGwAZgBWACkACgAgACAAewAKACAAIAAgACAAdQAgAD0AIAAtAHgALQB5AC0AaABhAGwAZgBWACAAKwB0AGkAbQBlADsACgAgACAAfQAKAH0ACgAKAAoACgByAGUAdAB1AHIAbgAgAGYAbABvAGEAdAAyACgAdQAgACwAIAB2ACkAOwA=,output:1,fname:Function_node_10,width:474,height:510,input:0,input:0,input:0,input_1_label:x,input_2_label:y,input_3_label:time|A-7-U,B-7-V,C-11-T;n:type:ShaderForge.SFN_Time,id:11,x:33195,y:32599;proporder:2;pass:END;sub:END;*/

Shader "Custom/HexagonalDoorMat" {
    Properties {
        _node_2 ("node_2", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _node_2; uniform float4 _node_2_ST;
            float2 Function_node_10( float x , float y , float time ){
            float u = x +time;
            float v = y;
            
            if(x > 0.5)
             u = x-time;
            
            float halfV = 1.0/3.0;
            
            
            if(x < 0.5)
            {
            	if(y < halfV)
              {
            	  u = x+y-halfV + time;
              }
              else if (y > 2.0*halfV)
            	{
            	 u = x-y-halfV + time;
            	}
            }
            else
            {
              if(y < halfV)
              {
                u = -x+y-halfV + time;
              } 
              else if (y > 2.0*halfV)
              {
                u = -x-y-halfV +time;
              }
            }
            
            
            
            return float2(u , v);
            }
            
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_7 = i.uv0;
                float4 node_11 = _Time + _TimeEditor;
                float2 node_10 = Function_node_10( node_7.r , node_7.g , node_11.g );
                float4 node_2 = tex2D(_node_2,TRANSFORM_TEX(node_10, _node_2));
                float3 emissive = node_2.rgb;
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_2.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
