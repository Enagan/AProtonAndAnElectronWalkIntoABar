// Shader created with Shader Forge Beta 0.31 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.31;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|emission-2-RGB,alpha-2-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33175,y:32915,ptlb:node_2,ptin:_node_2,tex:fe105c350fabd054d91c8c450f6719ec,ntxv:0,isnm:False|UVIN-7-OUT;n:type:ShaderForge.SFN_TexCoord,id:4,x:33958,y:32816,uv:0;n:type:ShaderForge.SFN_Multiply,id:5,x:33637,y:32756|A-8-OUT,B-4-U;n:type:ShaderForge.SFN_Multiply,id:6,x:33637,y:32946|A-4-V,B-9-OUT;n:type:ShaderForge.SFN_Append,id:7,x:33402,y:32915|A-5-OUT,B-6-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8,x:33955,y:32731,ptlb:xScale,ptin:_xScale,glob:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:9,x:33957,y:33022,ptlb:yScale,ptin:_yScale,glob:False,v1:1;proporder:2-9-8;pass:END;sub:END;*/

Shader "Shader Forge/JackedInEmission" {
    Properties {
        _node_2 ("node_2", 2D) = "white" {}
        _yScale ("yScale", Float ) = 1
        _xScale ("xScale", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
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
            uniform sampler2D _node_2; uniform float4 _node_2_ST;
            uniform float _xScale;
            uniform float _yScale;
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
                float2 node_4 = i.uv0;
                float2 node_7 = float2((_xScale*node_4.r),(node_4.g*_yScale));
                float4 node_2 = tex2D(_node_2,TRANSFORM_TEX(node_7, _node_2));
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
