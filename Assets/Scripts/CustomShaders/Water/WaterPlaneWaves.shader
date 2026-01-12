Shader "Custom/Water/WaterPlane with Waves"
{
    Properties
    {
        [Header(Thresholds)]
        _IntersectionThreshold("Intersction threshold", float) = 0
        _FoamThreshold("Foam threshold", float) = 0
        
        [Space(10)]
        [Header(Colors)]
        _ShallowWaterColor("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DeepWaterColor("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)

        [Space(10)]
        [Header(Waves)]
        [KeywordEnum(Grid, Pointy)] _WaveMode ("Wave Mode", Float) = 0.0
        _WaveSpeed("Speed", Float) = 0.5
        _WaveAmplitude("Amplitude", Float) = 0.25
        _WaveFrequency("Frequency", Float) = 1.0
        _WaveDirection("Direction", Range(-1.0, 1.0)) = 0
        _WaveNoise("Noise", Range(0, 1)) = 0.25
        _AmplitudeNoise("Amplitude Noise", Range(0, 1)) = 0.25
        
        [Space(10)]
        [Header(SURFACE NOISE)]
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
        [Header(SURFACE DISTORTION)]
        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}
        _SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.27

        [Space(10)]
        [Header(FOAM)]
        _FoamColor("Foam Color", Color) = (1,1,1,1)
        _FoamStep("Foam Step", Range(0, 1)) = 0.5

        [Header(CAUSTICS)]
        _CausticsTex_A("Caustics Texture A", 2D) = "white" {}
        _Caustics_A_ST("Caustics Scale/Offset A ", Vector) = (1,1,0,0)
        _CausticsTex_B("Caustics Texture B", 2D) = "white" {}
        _Caustics_B_ST("Caustics Scale/Offset B ", Vector) = (1,1,0,0)
        _CausticsSpeed("Caustics Speed", float) = 1
        
        [Header(DISTANCE FADE)]
        [Toggle]_DistanceFadeEnabled("Enable Distance Fade", float) = 0
        _DistanceFadeThreshold("Distance Fade Threshold", float) = 0
    }

    SubShader
    {

        Tags
        {
            "Queue"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature _ALPHAPREMULTIPLY_ON

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                float4 positionNDC : TEXCOORD2;
                float2 uv : TEXCOORD3;
                float2 noiseUV : TEXCOORD4;
                float2 distortionUV : TEXCOORD5;
                float waveHeight : TEXCOORD6;
            };
            
            TEXTURE2D(_SurfaceNoise);
            SAMPLER(sampler_SurfaceNoise);
 
            TEXTURE2D(_SurfaceDistortion);
            SAMPLER(sampler_SurfaceDistortion);

            TEXTURE2D(_CausticsTex_A);
            SAMPLER(sampler_CausticsTex_A);
            
            TEXTURE2D(_CausticsTex_B);
            SAMPLER(sampler_CausticsTex_B);

            TEXTURE2D_X_FLOAT(_CustomDepthTexture);
            SAMPLER(sampler_CustomDepthTexture);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _DeepWaterColor;
            float4 _ShallowWaterColor;

            half _WaveMode;
            half _WaveFrequency;
            half _WaveAmplitude;
            half _WaveSpeed;
            half _WaveDirection;
            half _WaveNoise;
            half _AmplitudeNoise;

            float _IntersectionThreshold;
            float _FoamThreshold;

            half4 _SurfaceNoise_ST;
            half2 _SurfaceNoiseScroll;

            float4 _SurfaceDistortion_ST;
            half _SurfaceDistortionAmount;
            
            float4 _FoamColor;
            float _FoamStep;
            
            half4 _Caustics_A_ST;
            half4 _Caustics_B_ST;
            half _CausticsSpeed;

            int _DistanceFadeEnabled;
            half _DistanceFadeThreshold;
            
            int _WorldBendingDisabled;
            int _FogDisabled;
            CBUFFER_END

            #include "Assets/Scripts/CustomShaders/CommonFunctions.hlsl"

            float2 GradientNoise_Dir(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                // 3d0a9085-1fec-441a-bba6-f1121cdbe3ba
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float GradientNoise(float2 UV, float Scale)
            {
                const float2 p = UV * Scale;
                const float2 ip = floor(p);
                float2 fp = frac(p);
                const float d00 = dot(GradientNoise_Dir(ip), fp);
                const float d01 = dot(GradientNoise_Dir(ip + float2(0, 1)), fp - float2(0, 1));
                const float d10 = dot(GradientNoise_Dir(ip + float2(1, 0)), fp - float2(1, 0));
                const float d11 = dot(GradientNoise_Dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }

            inline float SineWave(float3 pos, float offset)
            {
                return sin(
                    offset + _Time.z * _WaveSpeed + (pos.x * sin(offset + _WaveDirection * PI) + pos.z *
                        cos(offset + _WaveDirection * PI)) * _WaveFrequency);
            }

            float Noise(float2 texcoord, half noise)
            {
                float2 noise_uv = texcoord * _WaveFrequency;
                float noise01 = GradientNoise(noise_uv, 1.0);
                return (noise01 * 2.0 - 1.0) * noise;
            }

            inline float WaveHeight(float2 texcoord, float3 position)
            {
                float noise = Noise(texcoord, _WaveNoise);
                float s = SineWave(position, noise);

                if (_WaveMode == 0)
                {
                    s *= SineWave(position, HALF_PI + noise);
                }
                if (_WaveMode == 1)
                {
                    s = 1.0 - abs(s);
                }
                return s;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                float3 worldBendFactor = WorldBendFactor(input.positionOS, _WorldBendingDisabled);
                input.positionOS.xyz += worldBendFactor;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);
                half3 originalPositionWS = vertexInput.positionWS;
                const float s = WaveHeight(input.uv, originalPositionWS);
                output.waveHeight = s;
                output.positionWS = originalPositionWS;
                float noise = Noise(input.uv, _AmplitudeNoise);
                output.positionWS.y += s * (_WaveAmplitude * noise);
                output.positionOS = input.positionOS; 
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.positionNDC = ComputeScreenPos(output.positionCS);
      
                output.uv = input.uv;
                output.noiseUV = TRANSFORM_TEX(input.uv, _SurfaceNoise);
                output.distortionUV = TRANSFORM_TEX(input.uv, _SurfaceDistortion);
                return output;
            }

            float2 sampleDepth(float4 positionNDC)
            {
                half2 screenUv = positionNDC.xy / positionNDC.w;
                half depthTextureSample = SAMPLE_TEXTURE2D_X(_CustomDepthTexture, sampler_CustomDepthTexture, UnityStereoTransformScreenSpaceTex(screenUv));
                half depth = LinearEyeDepth(depthTextureSample, _ZBufferParams);
                half fragmentDepth = positionNDC.w;
                
                half intersectionDiff = saturate((depth - fragmentDepth) / _IntersectionThreshold);
                half foamDiff = saturate((depth - fragmentDepth) / _FoamThreshold);
                return half2(intersectionDiff, foamDiff);
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                half2 depth = CustomDepthTextureDisabled > 0 ? float2(1,1) : sampleDepth(input.positionNDC);

                half2 distortSample = SAMPLE_TEXTURE2D(_SurfaceDistortion, sampler_SurfaceDistortion, input.distortionUV).xy * _SurfaceDistortionAmount;

                half2 noiseUV = float2(( input.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x,
                                        ( input.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);

                half causticsUvProgress = frac(_Time.y * _CausticsSpeed); ; 
                half2 uv = noiseUV * (_Caustics_A_ST.xy + _Caustics_A_ST.zw) + causticsUvProgress;
                half3 caustics = SAMPLE_TEXTURE2D(_CausticsTex_A, sampler_CausticsTex_A, uv).rgb;
                uv = noiseUV * (_Caustics_B_ST.xy + _Caustics_B_ST.zw) + causticsUvProgress;
                half3 causticsB = SAMPLE_TEXTURE2D(_CausticsTex_B, sampler_CausticsTex_B, uv).rgb;;
                half4 minCaustics = float4(min(causticsB, caustics).rgb, 0);
                
                half4 waterCol = lerp(_ShallowWaterColor, _DeepWaterColor, depth.x);
                waterCol = depth.y < _FoamStep ? _FoamColor : waterCol;

                half viewDistance = length(_WorldSpaceCameraPos - input.positionWS); 
                half distanceThreshold = 1 - saturate(viewDistance / _DistanceFadeThreshold);

                half4 surfaceNoiseSample =  SAMPLE_TEXTURE2D(_SurfaceNoise, sampler_SurfaceNoise, noiseUV);
                half4 waterBlend = AlphaBlend(surfaceNoiseSample, waterCol);
                waterBlend = _DistanceFadeEnabled > 0 ? lerp(waterCol, waterBlend, distanceThreshold)  : waterBlend;

                half4 waterWithCaustics = waterBlend + minCaustics;
                waterWithCaustics =_DistanceFadeEnabled > 0 ? lerp(waterBlend, waterWithCaustics, distanceThreshold) : waterWithCaustics;
                waterWithCaustics.rgb = _FogDisabled > 0 ? waterWithCaustics.rgb : FogColor(waterWithCaustics.rgb, input.positionWS);
                return waterWithCaustics;
            }
            ENDHLSL
        }
    }
}