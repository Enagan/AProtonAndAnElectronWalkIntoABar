// Shader created with Shader Forge Beta 0.32 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.32;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:False,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32045,y:32577|diff-2-RGB,emission-117-OUT,alpha-119-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:32755,y:32444,tex:fe105c350fabd054d91c8c450f6719ec,ntxv:0,isnm:False|UVIN-35-UVOUT,TEX-23-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:23,x:32944,y:32586,ptlb:texture,ptin:_texture,glob:False,tex:fe105c350fabd054d91c8c450f6719ec;n:type:ShaderForge.SFN_ValueProperty,id:32,x:33824,y:33046,ptlb:xOffset,ptin:_xOffset,glob:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:33,x:33824,y:33163,ptlb:yOffset,ptin:_yOffset,glob:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:35,x:33482,y:32541,uv:0;n:type:ShaderForge.SFN_Code,id:84,x:32676,y:33036,code:CQAKAGYAbABvAGEAdAAgAGEAbABwAGgAYQA7AAoACgBmAGwAbwBhAHQAMgAgAGQAaQByAFYAZQBjACAAPQAgAGYAbABvAGEAdAAyACgAWAAgAC0AIAAwAC4ANQBmACAALQAgAGMAZQBuAHQAZQByAE8AZgBmAHMAZQB0AC4AeAAsACAAWQAtADAALgA1AGYAIAAtACAAYwBlAG4AdABlAHIATwBmAGYAcwBlAHQALgB5ACkAOwAKAGYAbABvAGEAdAAgAGQAaQBzAHQAIAA9ACAAcwBxAHIAdAAoAGQAaQByAFYAZQBjAC4AeAAqAGQAaQByAFYAZQBjAC4AeAArAGQAaQByAFYAZQBjAC4AeQAqAGQAaQByAFYAZQBjAC4AeQApADsACgBmAGwAbwBhAHQAIABhAGwAcABoAGEARABpAHMAdAAgAD0AIABvAHUAdABlAHIAUgBhAGQAaQB1AHMAIAAtACAAcgBhAGQAaQB1AHMAOwAKAAoAaQBmACAAKAAgAGQAaQBzAHQAIAA8ACAAcgBhAGQAaQB1AHMAIAApAAoAewAKACAAIABhAGwAcABoAGEAIAA9ACAAMQAuADAAZgA7ACAAIAAJAAoAfQAKAGUAbABzAGUAIABpAGYAIAAoACAAZABpAHMAdAAgAD4AIABvAHUAdABlAHIAUgBhAGQAaQB1AHMAKQAKAHsACgAgACAAYQBsAHAAaABhACAAPQAgADAALgAwAGYAOwAKAH0ACgBlAGwAcwBlAAoAewAKACAAIABhAGwAcABoAGEAIAA9ACAAMQAgAC0AIAAoAGQAaQBzAHQAIAAtACAAcgBhAGQAaQB1AHMAKQAvACAAYQBsAHAAaABhAEQAaQBzAHQAOwAKAH0ACgAKAHIAZQB0AHUAcgBuACAAYQBsAHAAaABhADsA,output:0,fname:UVCircleAlpha,width:715,height:397,input:0,input:0,input:0,input:0,input:1,input_1_label:X,input_2_label:Y,input_3_label:radius,input_4_label:outerRadius,input_5_label:centerOffset|A-35-U,B-35-V,C-94-OUT,D-96-OUT,E-98-OUT;n:type:ShaderForge.SFN_ValueProperty,id:94,x:33476,y:32910,ptlb:innerRadius,ptin:_innerRadius,glob:False,v1:0.4;n:type:ShaderForge.SFN_ValueProperty,id:96,x:33476,y:32993,ptlb:outerRadius,ptin:_outerRadius,glob:False,v1:0.55;n:type:ShaderForge.SFN_Multiply,id:97,x:32646,y:32800|A-2-A,B-84-OUT;n:type:ShaderForge.SFN_Append,id:98,x:33567,y:33065|A-32-OUT,B-33-OUT;n:type:ShaderForge.SFN_Multiply,id:117,x:32417,y:32699|A-2-RGB,B-97-OUT;n:type:ShaderForge.SFN_Add,id:118,x:32470,y:32925|A-97-OUT,B-2-A;n:type:ShaderForge.SFN_Clamp01,id:119,x:32314,y:32925|IN-118-OUT;proporder:23-32-33-94-96;pass:END;sub:END;*/

Shader "Shader Forge/JackedInFloor" {
    Properties {
        _texture ("texture", 2D) = "white" {}
        _xOffset ("xOffset", Float ) = 0
        _yOffset ("yOffset", Float ) = 0
        _innerRadius ("innerRadius", Float ) = 0.4
        _outerRadius ("outerRadius", Float ) = 0.55
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
            uniform float4 _LightColor0;
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float _xOffset;
            uniform float _yOffset;
            float UVCircleAlpha( float X , float Y , float radius , float outerRadius , float2 centerOffset ){
            	
            float alpha;
            
            float2 dirVec = float2(X - 0.5f - centerOffset.x, Y-0.5f - centerOffset.y);
            float dist = sqrt(dirVec.x*dirVec.x+dirVec.y*dirVec.y);
            float alphaDist = outerRadius - radius;
            
            if ( dist < radius )
            {
              alpha = 1.0f;  	
            }
            else if ( dist > outerRadius)
            {
              alpha = 0.0f;
            }
            else
            {
              alpha = 1 - (dist - radius)/ alphaDist;
            }
            
            return alpha;
            }
            
            uniform float _innerRadius;
            uniform float _outerRadius;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
////// Emissive:
                float2 node_35 = i.uv0;
                float4 node_2 = tex2D(_texture,TRANSFORM_TEX(node_35.rg, _texture));
                float node_97 = (node_2.a*UVCircleAlpha( node_35.r , node_35.g , _innerRadius , _outerRadius , float2(_xOffset,_yOffset) ));
                float3 emissive = (node_2.rgb*node_97);
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * node_2.rgb;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,saturate((node_97+node_2.a)));
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float _xOffset;
            uniform float _yOffset;
            float UVCircleAlpha( float X , float Y , float radius , float outerRadius , float2 centerOffset ){
            	
            float alpha;
            
            float2 dirVec = float2(X - 0.5f - centerOffset.x, Y-0.5f - centerOffset.y);
            float dist = sqrt(dirVec.x*dirVec.x+dirVec.y*dirVec.y);
            float alphaDist = outerRadius - radius;
            
            if ( dist < radius )
            {
              alpha = 1.0f;  	
            }
            else if ( dist > outerRadius)
            {
              alpha = 0.0f;
            }
            else
            {
              alpha = 1 - (dist - radius)/ alphaDist;
            }
            
            return alpha;
            }
            
            uniform float _innerRadius;
            uniform float _outerRadius;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_35 = i.uv0;
                float4 node_2 = tex2D(_texture,TRANSFORM_TEX(node_35.rg, _texture));
                finalColor += diffuseLight * node_2.rgb;
                float node_97 = (node_2.a*UVCircleAlpha( node_35.r , node_35.g , _innerRadius , _outerRadius , float2(_xOffset,_yOffset) ));
/// Final Color:
                return fixed4(finalColor * saturate((node_97+node_2.a)),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
