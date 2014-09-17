// Shader created with Shader Forge Beta 0.32 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.32;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32644,y:32488|emission-98-OUT,alpha-179-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33674,y:32653,ptlb:tex1,ptin:_tex1,tex:d884fc085d1eefd499ed8efbb41b3f97,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3,x:33631,y:32233,ptlb:tex2,ptin:_tex2,tex:24f8ab53540613a4195932b67298e06d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Time,id:4,x:33951,y:32865;n:type:ShaderForge.SFN_Divide,id:49,x:33675,y:32952|A-4-T,B-52-OUT;n:type:ShaderForge.SFN_Vector1,id:52,x:33951,y:33056,v1:3;n:type:ShaderForge.SFN_Code,id:98,x:33104,y:32369,code:aQBmACgAcwBlAGwAZQBjAHQAIAA9AD0AIAAwACkACgAgACAAcgBlAHQAdQByAG4AIABUAGUAeAAxADsACgBlAGwAcwBlAAoAIAAgAHIAZQB0AHUAcgBuACAAVABlAHgAMgA7AA==,output:2,fname:Function_node_98,width:247,height:132,input:2,input:2,input:0,input_1_label:Tex1,input_2_label:Tex2,input_3_label:select|A-3-RGB,B-2-RGB,C-108-OUT;n:type:ShaderForge.SFN_Code,id:104,x:33184,y:32609,code:aQBmACgAcwBlAGwAZQBjAHQAIAA9AD0AMAApAAoAIAAgAHIAZQB0AHUAcgBuACAAYQBsAHAAaABhADEAOwAKAGUAbABzAGUACgAgACAAcgBlAHQAdQByAG4AIABhAGwAcABoAGEAMgA7AA==,output:0,fname:Function_node_104,width:247,height:132,input:0,input:0,input:0,input_1_label:alpha1,input_2_label:alpha2,input_3_label:select|A-3-A,B-2-A,C-108-OUT;n:type:ShaderForge.SFN_ValueProperty,id:108,x:33682,y:32464,ptlb:selectTex,ptin:_selectTex,glob:False,v1:0;n:type:ShaderForge.SFN_Sin,id:172,x:33465,y:32940|IN-49-OUT;n:type:ShaderForge.SFN_Multiply,id:179,x:32975,y:32828|A-104-OUT,B-201-OUT;n:type:ShaderForge.SFN_Code,id:201,x:33156,y:32860,code:aQBmACgAcwBlAGwAZQBjAHQAIAA9AD0AMAApAAoAIAAgAHIAZQB0AHUAcgBuACAAdABpAG0AZQA7AAoAZQBsAHMAZQAKACAAIAByAGUAdAB1AHIAbgAgADEALQAgAHQAaQBtAGUAOwA=,output:0,fname:Function_node_201,width:247,height:132,input:0,input:0,input_1_label:select,input_2_label:time|A-108-OUT,B-172-OUT;proporder:2-3-108;pass:END;sub:END;*/

Shader "Custom/JackedInParticles" {
    Properties {
        _tex1 ("tex1", 2D) = "white" {}
        _tex2 ("tex2", 2D) = "white" {}
        _selectTex ("selectTex", Float ) = 0
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
            uniform sampler2D _tex1; uniform float4 _tex1_ST;
            uniform sampler2D _tex2; uniform float4 _tex2_ST;
            float3 Function_node_98( float3 Tex1 , float3 Tex2 , float select ){
            if(select == 0)
              return Tex1;
            else
              return Tex2;
            }
            
            float Function_node_104( float alpha1 , float alpha2 , float select ){
            if(select ==0)
              return alpha1;
            else
              return alpha2;
            }
            
            uniform float _selectTex;
            float Function_node_201( float select , float time ){
            if(select ==0)
              return time;
            else
              return 1- time;
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
                float2 node_251 = i.uv0;
                float4 node_3 = tex2D(_tex2,TRANSFORM_TEX(node_251.rg, _tex2));
                float4 node_2 = tex2D(_tex1,TRANSFORM_TEX(node_251.rg, _tex1));
                float3 emissive = Function_node_98( node_3.rgb , node_2.rgb , _selectTex );
                float3 finalColor = emissive;
                float4 node_4 = _Time + _TimeEditor;
                float node_172 = sin((node_4.g/3.0));
/// Final Color:
                return fixed4(finalColor,(Function_node_104( node_3.a , node_2.a , _selectTex )*Function_node_201( _selectTex , node_172 )));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
