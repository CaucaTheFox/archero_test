Shader "Hidden/Mask Stencil"
{
    Properties
    {
        _Stencil("Stencil Reference Value", Range(0, 255)) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        Pass
        {
            Blend Zero One
            ZWrite Off

            Stencil
            {
                 Ref [_Stencil]
                 Comp [_StencilComp]
                 Pass [_StencilOp]
                 ReadMask [_StencilReadMask]
                 WriteMask [_StencilWriteMask]
            }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag

            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                return output;
            }

            half4 frag(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
}