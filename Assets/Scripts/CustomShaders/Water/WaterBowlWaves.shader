Shader "Custom/Water/WaterBowl with Waves"
{
    Properties
    {
        [Header(WATER)]
        _WaterHeight ("Water height", Float) = 0
        _WaterHeightDistance("Water Height Distance", Float) = 0
        _DistanceStep("Water Height Distance step", Range(0,1)) = 0.5
        _MaxWaterDepth("Max Water Depth", Float) = 0
        
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
        [Header(DEPTH)]
        _ShallowWaterColor("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DeepWaterColor("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)

        [Space(10)]
        [Header(SURFACE NOISE)]
        _WaterPlaneUvScalarNoise("Noise Uv Scalar", Float) = 0
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
        [Header(SURFACE DISTORTION)]
        _WaterPlaneUvScalarDistortion("Distortion Uv Scalar", Float) = 0
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
    }

    SubShader
    {

        Tags
        {
            "Queue"="AlphaTest"
            "RenderType" = "TransparentCutout"
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
                float3 positionWS : TEXCOORD1;
                float4 camHeightOverWater : TEXCOORD2;
                float4 waterDepth : TEXCOORD3;
                float waterHeight : TEXCOORD4;
            };


            TEXTURE2D(_SurfaceNoise);
            SAMPLER(sampler_SurfaceNoise);
 
            TEXTURE2D(_SurfaceDistortion);
            SAMPLER(sampler_SurfaceDistortion);

            TEXTURE2D(_CausticsTex_A);
            SAMPLER(sampler_CausticsTex_A);
            
            TEXTURE2D(_CausticsTex_B);
            SAMPLER(sampler_CausticsTex_B);

            CBUFFER_START(UnityPerMaterial)
            float _WaterPlaneUvScalarNoise;
            
            float4 _DeepWaterColor;
            half _FresnelPower;

            half4 _SurfaceNoise_ST;
            half2 _SurfaceNoiseScroll;

            float _WaterPlaneUvScalarDistortion;
            float4 _SurfaceDistortion_ST;
            half _SurfaceDistortionAmount;
            float _WaterHeight;
            half4 _ShallowWaterColor;
            float _FoamStep;
            float _WaterHeightWaveScalar;
            float _WaterHeightDistance;
            float _DistanceStep;
            float _MaxWaterDepth;

            half _WaveMode;
            half _WaveFrequency;
            half _WaveAmplitude;
            half _WaveSpeed;
            half _WaveDirection;
            half _WaveNoise;
            half _AmplitudeNoise;
            
            float4 _FoamColor;
            half4 _Caustics_A_ST;
            half4 _Caustics_B_ST;
            half _CausticsSpeed;

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
            
            float2 WaterPlaneUV(float3 worldPos, float camHeightOverWater)
            {
                float3 camToWorldRay = worldPos - _WorldSpaceCameraPos;
                float3 rayToWaterPlane = (camHeightOverWater / camToWorldRay.y * camToWorldRay);
                float2 uv = rayToWaterPlane.xz - _WorldSpaceCameraPos.xz;
                return uv;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;

                float3 worldBendFactor = WorldBendFactor(input.positionOS, _WorldBendingDisabled);
                input.positionOS.xyz += worldBendFactor;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);
                half3 originalPositionWS = vertexInput.positionWS;
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                const float s = WaveHeight(input.uv, originalPositionWS);
                float noise = Noise(input.uv, _AmplitudeNoise);
                float waterHeight = _WaterHeight += s * (_WaveAmplitude * noise);
                output.waterHeight = waterHeight - length(worldBendFactor);
                float3 camToWorldRay = output.positionWS - _WorldSpaceCameraPos;
                output.camHeightOverWater = _WorldSpaceCameraPos.y - output.waterHeight;

                float3 rayToWaterPlane = output.camHeightOverWater / (-camToWorldRay.y) * camToWorldRay;
                float depth = length(camToWorldRay - rayToWaterPlane);
                output.waterDepth = depth * saturate(rayToWaterPlane.y - camToWorldRay.y);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float waterHeight = sin(_Time.y + input.waterHeight * _WaterHeightWaveScalar) * 0.2 + input.waterHeight;
                float waterDepth = max(0, waterHeight - input.positionWS.y);
                float depthNormalized = saturate(waterDepth / _MaxWaterDepth);
                float useWaterColor = lerp(0, 1, waterDepth > 0);

                float2 planeUv = WaterPlaneUV(input.positionWS, input.camHeightOverWater);
                float2 planeUvNoise = planeUv * _WaterPlaneUvScalarNoise;
                float2 planeDistortionUv = planeUv * _WaterPlaneUvScalarDistortion;
                half2 distortSample = SAMPLE_TEXTURE2D(_SurfaceDistortion, sampler_SurfaceDistortion, planeDistortionUv).xy * _SurfaceDistortionAmount;
                half2 noiseUV = float2((planeUvNoise.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x,
                                       (planeUvNoise.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);

                half4 surfaceNoiseSample =  SAMPLE_TEXTURE2D(_SurfaceNoise, sampler_SurfaceNoise, noiseUV);

                float2 uv = noiseUV * _Caustics_A_ST.xy + _Caustics_A_ST.zw;
                uv += _CausticsSpeed * _Time.y;
                float3 caustics = SAMPLE_TEXTURE2D(_CausticsTex_A, sampler_CausticsTex_A, uv).rgb;

                float2 uvB = noiseUV * _Caustics_B_ST.xy + _Caustics_B_ST.zw;
                uvB += _CausticsSpeed * _Time.y;
                float3 causticsB = SAMPLE_TEXTURE2D(_CausticsTex_B, sampler_CausticsTex_B, uv).rgb;;

                float4 waterCol = lerp(_ShallowWaterColor, _DeepWaterColor, depthNormalized);
                waterCol = depthNormalized < _FoamStep ? _FoamColor : waterCol;

                float4 minCaustics = float4(min(causticsB, caustics).rgb, 0);

                float4 waterBlend = AlphaBlend(surfaceNoiseSample, waterCol);
                float4 waterWithCaustics = waterBlend + minCaustics;
                return useWaterColor ? waterWithCaustics : float4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }
}