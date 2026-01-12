Shader "Hidden/DepthOfField RenderFeature"
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

        ZTest Always 
        Cull Off
        ZWrite Off
        
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
            float4 uv2 : TEXCOORD1;
        };

        TEXTURE2D(_BlitTexture); SAMPLER(sampler_BlitTexture);
        TEXTURE2D(_BlurTex); SAMPLER(sampler_BlurTex);
        TEXTURE2D_X_FLOAT(_CustomDepthTexture); SAMPLER(sampler_CustomDepthTexture);

        int _VisualizeLens;
        half _BlurStrength;
        half _Focus;
        half _Aperture;
        
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
            
            half2 offset = _BlitTexture_TexelSize.xy * _BlurStrength.xx * (1.0h / _BlitTexture_ST.xy);
            half2 offsetUv = uv - offset;
            half2 offsetUv2 = uv + offset;
            output.uv2 = half4(offsetUv, offsetUv2);
            return output;
        }
       
        ENDHLSL

        Pass
        {
            Name "BLUR"
            HLSLPROGRAM
            half4 frag(Varyings input) : SV_TARGET
            {
                half4 c = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv);
                c += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv2.xy);
                c += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv2.xw);
                c += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv2.zy);
                c += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv2.zw);
                return c * 0.2h;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DEPTH OF FIELD"
            HLSLPROGRAM
            half4 frag(Varyings input) : SV_TARGET
            {
                half4 mainSample = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.uv);
                half4 blurSample = SAMPLE_TEXTURE2D(_BlurTex, sampler_BlurTex, input.uv);	
                half depthTextureSample = SAMPLE_TEXTURE2D_X(_CustomDepthTexture, sampler_CustomDepthTexture, input.uv);

                half depth = LinearEyeDepth(depthTextureSample, _ZBufferParams);
                half mask = saturate(abs((1.0h - clamp(max(depth / _Focus, _Focus / depth), 0.0h, 20.0h)) * _Aperture)); 
                return _VisualizeLens > 0
                ? lerp(half4(1, 1, 1, 1), blurSample, mask)
                :lerp(mainSample, blurSample, mask);
            }
            ENDHLSL
        }
    }
}