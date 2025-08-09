Shader "Hidden/Bloom RenderFeature"
{
    Properties
    {
        _BlitTexture ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Cull Back
        ZTest Off
        ZWrite Off
            
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_BlitTexture); SAMPLER(sampler_BlitTexture);
        TEXTURE2D(_SourceTex); SAMPLER(sampler_SourceTex);
        
        CBUFFER_START(UnityPerMaterial)
        half4 _BlitTexture_TexelSize;
        half4 _Filter;
        CBUFFER_END

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            uint vertexID : SV_VertexID;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        half3 Sample(float2 uv)
        {
            return SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv).rgb;
        }

        half3 SampleBox(float2 uv, float delta)
        {
            half4 o = _BlitTexture_TexelSize.xyxy * half2(-delta, delta).xxyy;
            half3 s =
                Sample(uv + o.xy) + Sample(uv + o.zy) +
                Sample(uv + o.xw) + Sample(uv + o.zw);
            return s * 0.25f;
        }

        half3 Prefilter(half3 c)
        {
            half brightness = max(c.r, max(c.g, c.b));
            half soft = brightness - _Filter.y;
            soft = clamp(soft, 0, _Filter.z);
            soft = soft * soft * _Filter.w;
            half contribution = max(soft, brightness - _Filter.x);
            contribution /= max(brightness, 0.00001);
            return c * saturate(contribution);
        }

        Varyings vert(Attributes input)
        {
            Varyings output;
            
             #if SHADER_API_GLES
                float4 pos = input.positionOS;
                float2 uv  = input.uv;
            #else
                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);
            #endif
            
            output.positionCS = pos;
            output.uv   = uv;
            return output;
        }
        ENDHLSL

        Pass
        {
            Name "BoxDownPrefilterPass"
            Cull Back
            ZTest Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                return half4(Prefilter(SampleBox(input.uv, 1)), 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "BoxDownPass"
            ZTest Off
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                return half4(SampleBox(input.uv, 1), 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "BoxUpPass"
            Cull Back
            ZTest Off
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                return half4(SampleBox(input.uv, 0.5), 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "AdditiveBoxUpPass"
            Cull Back
            ZTest Off
            ZWrite Off
            Blend One One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                return half4(SampleBox(input.uv, 0.5), 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ApplyBloomPass"
            Cull Back
            ZTest Off
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(Varyings input) : SV_Target
            {
                half4 result = SAMPLE_TEXTURE2D(_SourceTex, sampler_SourceTex, input.uv) + half4(SampleBox(input.uv, 0.5), 1);
                result.a = 1; 
                return result;
            }
            ENDHLSL
        }
    }
}