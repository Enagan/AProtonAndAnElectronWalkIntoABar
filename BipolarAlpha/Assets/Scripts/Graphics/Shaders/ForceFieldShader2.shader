// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:2,bsrc:0,bdst:0,culm:2,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32488,y:33033|emission-135-OUT,alpha-54-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33592,y:32561,ptlb:Line,ptin:_Line,tex:85c805f172482554e827f791d1c5af8a,ntxv:0,isnm:False|UVIN-143-OUT;n:type:ShaderForge.SFN_Multiply,id:4,x:33373,y:32599|A-2-RGB,B-6-OUT;n:type:ShaderForge.SFN_OneMinus,id:5,x:33648,y:32886|IN-6-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6,x:33730,y:32750,ptlb:NoiseBlend,ptin:_NoiseBlend,glob:False,v1:0.85;n:type:ShaderForge.SFN_Multiply,id:7,x:33355,y:32825|A-5-OUT,B-167-OUT;n:type:ShaderForge.SFN_Add,id:8,x:33178,y:32728|A-4-OUT,B-7-OUT;n:type:ShaderForge.SFN_TexCoord,id:15,x:34398,y:32875,uv:0;n:type:ShaderForge.SFN_Panner,id:36,x:34095,y:32759,spu:1,spv:1|UVIN-15-UVOUT,DIST-69-OUT;n:type:ShaderForge.SFN_Time,id:40,x:34330,y:33113;n:type:ShaderForge.SFN_ValueProperty,id:53,x:33380,y:33277,ptlb:BaseAlpha,ptin:_BaseAlpha,glob:False,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:54,x:32974,y:33135|A-53-OUT,B-57-OUT;n:type:ShaderForge.SFN_Multiply,id:55,x:33380,y:33084|A-5-OUT,B-167-OUT;n:type:ShaderForge.SFN_Multiply,id:56,x:33380,y:32958|A-6-OUT,B-2-A;n:type:ShaderForge.SFN_Add,id:57,x:33209,y:32960|A-56-OUT,B-55-OUT;n:type:ShaderForge.SFN_ValueProperty,id:68,x:34298,y:33049,ptlb:TimeScale,ptin:_TimeScale,glob:False,v1:0.15;n:type:ShaderForge.SFN_Multiply,id:69,x:34115,y:33003|A-68-OUT,B-40-T;n:type:ShaderForge.SFN_Color,id:76,x:33167,y:32449,ptlb:Color,ptin:_Color,glob:False,c1:0.1529412,c2:0.7921569,c3:0.9568627,c4:1;n:type:ShaderForge.SFN_Multiply,id:77,x:32950,y:32500|A-76-RGB,B-8-OUT;n:type:ShaderForge.SFN_ValueProperty,id:88,x:33143,y:32351,ptlb:Brightness,ptin:_Brightness,glob:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:89,x:32779,y:32411|A-88-OUT,B-77-OUT;n:type:ShaderForge.SFN_Multiply,id:135,x:32751,y:32785|A-89-OUT,B-54-OUT;n:type:ShaderForge.SFN_Panner,id:140,x:34120,y:33316,spu:1,spv:0.5|UVIN-15-UVOUT,DIST-69-OUT;n:type:ShaderForge.SFN_Append,id:143,x:33920,y:32547|A-15-U,B-148-OUT;n:type:ShaderForge.SFN_ComponentMask,id:148,x:33920,y:32759,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-36-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:164,x:33989,y:33178,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-140-UVOUT;n:type:ShaderForge.SFN_Append,id:165,x:33825,y:33059|A-15-U,B-164-OUT;n:type:ShaderForge.SFN_ValueProperty,id:166,x:33971,y:33417,ptlb:NoiseScale,ptin:_NoiseScale,glob:False,v1:2;n:type:ShaderForge.SFN_Noise,id:167,x:33593,y:33233|XY-173-OUT;n:type:ShaderForge.SFN_Multiply,id:173,x:33757,y:33260|A-165-OUT,B-166-OUT;proporder:2-6-53-68-76-88-166;pass:END;sub:END;*/

Shader "Custom/ForceFieldShader2" {
    Properties {
        _Line ("Line", 2D) = "white" {}
        _NoiseBlend ("NoiseBlend", Float ) = 0.85
        _BaseAlpha ("BaseAlpha", Float ) = 0.3
        _TimeScale ("TimeScale", Float ) = 0.15
        _Color ("Color", Color) = (0.1529412,0.7921569,0.9568627,1)
        _Brightness ("Brightness", Float ) = 3
        _NoiseScale ("NoiseScale", Float ) = 2
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
            Blend One One
            Cull Off
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Line; uniform float4 _Line_ST;
            uniform float _NoiseBlend;
            uniform float _BaseAlpha;
            uniform float _TimeScale;
            uniform float4 _Color;
            uniform float _Brightness;
            uniform float _NoiseScale;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_15 = i.uv0;
                float4 node_40 = _Time + _TimeEditor;
                float node_69 = (_TimeScale*node_40.g);
                float2 node_143 = float2(node_15.r,(node_15.rg+node_69*float2(1,1)).g);
                float4 node_2 = tex2D(_Line,TRANSFORM_TEX(node_143, _Line));
                float node_5 = (1.0 - _NoiseBlend);
                float2 node_173 = (float2(node_15.r,(node_15.rg+node_69*float2(1,0.5)).g)*_NoiseScale);
                float2 node_167_skew = node_173 + 0.2127+node_173.x*0.3713*node_173.y;
                float2 node_167_rnd = 4.789*sin(489.123*(node_167_skew));
                float node_167 = frac(node_167_rnd.x*node_167_rnd.y*(1+node_167_skew.x));
                float node_54 = (_BaseAlpha*((_NoiseBlend*node_2.a)+(node_5*node_167)));
                float3 node_135 = ((_Brightness*(_Color.rgb*((node_2.rgb*_NoiseBlend)+(node_5*node_167))))*node_54);
                float3 emissive = node_135;
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_54);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
