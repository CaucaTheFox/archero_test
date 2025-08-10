#ifndef LIGHTING_INCLUDED
#define LIGHTING_INCLUDED

// based on DustyRoom FlatKit shaders https://flatkit.dustyroom.com/

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

half NdotL(half3 normal, half3 lightDir) { 
    half NdotL = dot(normal, lightDir);
    half angleDiff = saturate((NdotL * 0.5 + 0.5) - _SelfShadingSize);
    half angleDiffTransition = smoothstep(0, _ShadowEdgeSize, angleDiff); 
    return lerp(angleDiff, angleDiffTransition, _Flatness);
}

half3 LightingPhysicallyBased_Toon(Light light, half3 normalWS)
{
    half4 baseColor = _BaseColor;
    half nDotL = NdotL(normalWS, light.direction);
    baseColor = lerp(_ColorShaded, baseColor, nDotL);
    baseColor.rgb *= light.color * light.distanceAttenuation;

    return baseColor.rgb;
}

void ApplyToonLight(inout Light light)
{
    const half shadowAttenuation = saturate(light.shadowAttenuation * _UnityShadowSharpness);
    light.shadowAttenuation = shadowAttenuation;

    const half distanceAttenuation = smoothstep(0, _LightFalloffSize + 0.001, light.distanceAttenuation);
    light.distanceAttenuation = distanceAttenuation;

    const half3 lightColor = lerp(half3(1, 1, 1), light.color, _LightContribution);
    light.color = lightColor;
}

half4 UniversalFragment_Toon(InputData inputData, half3 albedo, half3 emission, half alpha)
{
    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS,  half4(1, 1, 1, 1));
    
    ApplyToonLight(mainLight);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));
    
    BRDFData brdfData;
    InitializeBRDFData(albedo, 1.0 - 1.0 / kDieletricSpec.a, 0, 0, alpha, brdfData);
    half3 color = GlobalIllumination(brdfData, inputData.bakedGI, 1.0, inputData.normalWS, inputData.viewDirectionWS);
    color += LightingPhysicallyBased_Toon(mainLight, inputData.normalWS);
    
    color += emission;
    return half4(color, alpha);
}

#endif