Shader "Hidden/Blur RenderFeature"
{
    Properties
    {
        _BlitTexture ("Texture", 2D) = "white"
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

        TEXTURE2D(_BlitTexture); SAMPLER(sampler_BlitTexture);
        int _BlurStrength;
        
        CBUFFER_START(UnityPerMaterial)
        float4 _BlitTexture_ST;
        float4 _BlitTexture_TexelSize;
        CBUFFER_END
        
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
            Name "VERTICAL BOX BLUR"

            HLSLPROGRAM
            half4 frag(Varyings input) : SV_TARGET
            {
                half2 res = _BlitTexture_TexelSize.xy;
                half4 sum = 0;

                int samples = 2 * _BlurStrength + 1;

                for (half y = 0; y < samples; y++)
                {
                    half2 offset = half2(0, y - _BlurStrength);
                    sum += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv + offset * res);
                }
                return sum / samples;
            }
            ENDHLSL
        }

        Pass
        {
            Name "HORIZONTAL BOX BLUR"

            HLSLPROGRAM
            half4 frag(Varyings input) : SV_TARGET
            {
                float2 res = _BlitTexture_TexelSize.xy;
                half4 sum = 0;

                int samples = 2 * _BlurStrength + 1;

                for (float x = 0; x < samples; x++)
                {
                    float2 offset = float2(x - _BlurStrength, 0);
                    sum += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv + offset * res);
                }

                return sum / samples;
            }
            ENDHLSL
        }
    }
}