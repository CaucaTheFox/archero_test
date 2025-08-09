#ifndef COMMON_FUNCTIONS_INCLUDED
#define COMMON_FUNCTIONS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// --- Global World Bending Effect
float WorldBending;
half GlobalWorldBendingDisabled;

// --- Global Wind Effect
float3 WindDirection;
float3 WindSwayingMultiplier;

// --- Global Fog Effect
float FogIntensity;
half2 MinMaxFogDistance;
half GlobalFogDisabled;

// --- Custom Depth Texture
half CustomDepthTextureDisabled;

TEXTURE2D(NoiseTexture); SAMPLER(sampler_NoiseTexture);
TEXTURE2D(FogColorTex); SAMPLER(samplerFogColorTex);
TEXTURE2D(_FogBlendTexture); SAMPLER(sampler_FogBlendTexture);

float3 WorldBendFactor(float4 positionOS, half worldBendingDisabled)
{
    float4 worldPos = mul(unity_ObjectToWorld, positionOS);
    worldPos.xyz -= GetCameraPositionWS().xyz;
    worldPos = float4(0.0f, ((worldPos.z * worldPos.z) + (worldPos.x * worldPos.x)) * -WorldBending,
                      0.0f, 0.0f);
    float4 offsetWorldPos = mul(unity_WorldToObject, worldPos);
    float3 noOffset = float3(0,0,0);
    return GlobalWorldBendingDisabled > 0 ? noOffset : worldBendingDisabled > 0 ?  noOffset: offsetWorldPos.xyz;
}

float3 WorldBendFactor(float3 positionOS, half worldBendingDisabled)
{
    float3 worldPos = mul(unity_ObjectToWorld, positionOS).xyz;
    worldPos -= GetCameraPositionWS();
    worldPos = float4(0.0f, ((worldPos.z * worldPos.z) + (worldPos.x * worldPos.x)) * -WorldBending,
                      0.0f, 0.0f);
    float4 offsetObjectSpace = mul(unity_WorldToObject, worldPos);
    float3 noOffset = float3(0,0,0);
    return GlobalWorldBendingDisabled > 0 ? noOffset : worldBendingDisabled > 0 ?  noOffset : offsetObjectSpace.xyz;
}

float3 SwayingOffset(float4 positionOS, float4 color, half windStrength, half UIElementScalar, int isUiElement)
{
    float3 windDirection = float3((WindDirection).xz ,  0.0 );
    float3 worldPos = mul( unity_ObjectToWorld, positionOS).xyz;
    float2 panner = ( 1.0 * _Time.y * ( windDirection * 0.4 * 10.0 ).xy + (worldPos).xy);
    float2 noiseUv = (panner * 0.1) / float2( 10,10 ); 
    float4 windNoise =  SAMPLE_TEXTURE2D_LOD(NoiseTexture, sampler_NoiseTexture, noiseUv, 0) * windStrength * 0.8;
    float windInfluence = color.a > 0.1 ? color.a * windNoise +  color.g * windNoise : 0; 
    float4 windOffsetWS = mul(unity_WorldToObject, float4( WindDirection , 0.0 ) * windInfluence);
    float3 finalWindOffset = windOffsetWS.xyz * WindSwayingMultiplier; 
    return isUiElement > 0
    ? finalWindOffset / UIElementScalar
    : finalWindOffset;
}

float4 AlphaBlend(float4 top, float4 bottom)
{
    float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
    half alpha = top.a + bottom.a * (1 - top.a);
    return float4(color, alpha);
}

float3 FogColor(float3 color, float3 worldPos)
{
    float viewDistance = length(_WorldSpaceCameraPos - worldPos);
    float normalizedDistance = saturate((viewDistance - MinMaxFogDistance.x) / (MinMaxFogDistance.y - MinMaxFogDistance.x));
    float4 fogCol = SAMPLE_TEXTURE2D(FogColorTex, samplerFogColorTex, float2(normalizedDistance, 0));
    return GlobalFogDisabled > 0 ? color :  normalizedDistance < 1 ? lerp(color, fogCol, fogCol.a * FogIntensity).rgb : color;
}

float4 Caustics(float2 noiseUV, half4 texOffsetA, half4 texOffsetB, sampler2D texA, sampler2D texB, float speed)
{
    float2 uv = noiseUV * texOffsetA.xy + texOffsetA.zw;
    uv += speed * _Time.y;
    float3 caustics = tex2D(texA, uv).rgb;

    float2 uvB = noiseUV * texOffsetB.xy + texOffsetB.zw;
    uvB += speed * _Time.y;
    float3 causticsB = tex2D(texB, uvB).rgb;

    return float4(min(causticsB, caustics).rgb, 0);
}

float4 WaterColor(float4 color, float3 worldPos, float3 positionOS, half worldBendingDisabled,
    float waterHeight, float waterHeightWaveScalar, float4 shallowWaterColor, float foamStep)
{
    float3 worldBendingOffset = WorldBendFactor(positionOS, worldBendingDisabled);
    float3 worldBendingOffsetWS = mul(unity_ObjectToWorld, worldBendingOffset).xyz;
    float waterHeightNoWorldBending = waterHeight - worldBendingOffsetWS;
    float heightWithWave = sin(_Time.y + waterHeightNoWorldBending * waterHeightWaveScalar) * 0.2 + waterHeightNoWorldBending;
    float4 foamColor = float4(1,1,1,1);
    float distanceToWater = worldPos.y - heightWithWave;
    float4 waterblend = AlphaBlend( shallowWaterColor, color);
    return distanceToWater < 0
               ?  waterblend
               : distanceToWater < foamStep
               ? foamColor
               : color;
}

float4 ToGreyscale(half4 color, half brightness = 1)
{
    float grey = 0.21 * color.r +  0.71 * color.g + 0.07 * color.b;
    float4 greyColor = float4(grey, grey, grey, color.a);
    greyColor.rgb = lerp(greyColor.rgb, half3(1, 1, 1), brightness);
    return greyColor;
}

#endif 
