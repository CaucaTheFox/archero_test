Shader "Custom/ToonShader"
{
    Properties
    {
        [MainColor] _BaseColor ("Color", Color) = (1,1,1,1)
        _BaseMap("Main Texture", 2D) = "white" {}

        [Space(10)]
        _ColorShaded ("Color Shaded", Color) = (0.85023, 0.85034, 0.85045, 0.85056)
        _SelfShadingSize ("Self Shading Size", Range(0, 1)) = 0.5
        _ShadowEdgeSize ("Shadow Edge Size", Range(0, 0.5)) = 0.05
        _Flatness ("Flatness", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Name "ForwardLit"
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "LightMode" = "UniversalForward"
        }
        
        Cull Back
        
        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;

                float3 positionWS : TEXCOORD2;
                float4 positionOS : TEXCOORD3;

                float3 normal : TEXCOORD4;
                float3 viewDir : TEXCOORD5;

                float4 positionCS : SV_POSITION;
                float4 vertexColor : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half _Cutoff;
            half4 _BaseMap_ST;

            // --- _CELPRIMARYMODE_SINGLE
            half4 _ColorShaded;
            half _SelfShadingSize;
            half _ShadowEdgeSize;
            half _Flatness;
            
            // --- LIGHTING
            half _LightContribution;
            half _LightFalloffSize;
            half _UnityShadowPower;
            half _UnityShadowSharpness;
            half4 _UnityShadowColor;
            CBUFFER_END

            #include "Assets/Scripts/CustomShaders/Lighting.hlsl"
            #include "Assets/Scripts/CustomShaders/CommonFunctions.hlsl"

            void InitializeInputData(Varyings input, out InputData inputData)
            {
                inputData.positionWS = input.positionWS;

                half3 viewDirWS = input.viewDir;
                inputData.normalWS = input.normal;

                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                viewDirWS = SafeNormalize(viewDirWS);

                inputData.viewDirectionWS = viewDirWS;
                inputData.shadowCoord = float4(0, 0, 0, 0);
                inputData.fogCoord = 0;
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = half4(0, 0, 0, 0);

                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = half4(0, 0, 0, 0);

                inputData.positionCS = input.positionCS;
                inputData.tangentToWorld = half3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                const VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionWS.xyz = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.positionOS = input.positionOS;

                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
                output.viewDir = GetCameraPositionWS() - vertexInput.positionWS;
                return output;
            }

            half4 frag(Varyings input, half facing : VFACE) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                InputData inputData;
                InitializeInputData(input, inputData);

                // Computes direct light contribution.
                half4 shadedBaseColor = UniversalFragment_Toon(inputData, _BaseColor.rgb, half3(0, 0, 0), 1);
                half4 color = shadedBaseColor;

                half4 colorTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                color.rgb *= colorTex.rgb;
                color.a = colorTex.a;
                clip(color.a - _Cutoff);
                
                color.a = OutputAlpha(color.a, 1);
                
                return color;
            }
            ENDHLSL
        }
    }

    CustomEditor "CharacterShaderEditor"
}