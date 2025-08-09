Shader "Custom/Water/Water Bowl"
{
    Properties
    {
        [Header(WATER)]
        _WaterHeight ("Water height", Float) = 0
        _WaterHeightDistance("Water Height Distance", Float) = 0
        _DistanceStep("Water Height Distance step", Range(0,1)) = 0.5
        _MaxWaterDepth("Max Water Depth", Float) = 0
        _WaterHeightWaveScalar("Water Height Wave Scalar", Float) = 1
        [Space(10)]
        [Header(DEPTH)]
        _ShallowWaterColor("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DeepWaterColor("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)

        [Space(10)]
        [Header(SURFACE NOISE)]
        _WaterPlaneUvScalarNoise("Noise Uv Scalar", Float) = 0
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
        [Header(SURFACE NOISE)]
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
            
            float4 _FoamColor;
            half4 _Caustics_A_ST;
            half4 _Caustics_B_ST;
            half _CausticsSpeed;
            CBUFFER_END

            #include "Assets/Scripts/CustomShaders/CommonFunctions.hlsl"
            
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
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS);
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;

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