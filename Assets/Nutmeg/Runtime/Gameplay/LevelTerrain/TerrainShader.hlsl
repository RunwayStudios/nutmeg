﻿// Make sure this file is not included twice
#ifndef PYRAMIDFACES_INCLUDED
#define PYRAMIDFACES_INCLUDED

// Include helper functions from URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGGeometryHelpers.hlsl"


// This structure is created by the renderer and passed to the Vertex function
// It holds data stored on the model, per vertex
struct Attributes
{
    //float2 normalOS     : NORMAL;
    float3 positionOS : POSITION; // Position in object space
    //float2 uv           : TEXCOORD0; // UVs
};

// Other common semantics include NORMAL, TANGENT, COLOR

// This structure is generated by the vertex function and passed to the geometry function
struct VertexOutput
{
    float3 positionWS : TEXCOORD0; // Position in world space
    //half4 fogFactorAndVertexLight : TEXCOORD1; // x: fogFactor, yzw: vertex light
    //float2 uv           : TEXCOORD1; // UVs
};

// This structure is generated by the geometry function and passed to the fragment function
// Remember the renderer averages these values between the three points on the triangle 
struct GeometryOutput
{
    float3 positionWS : TEXCOORD0; // Position in world space
    half3 normalWS : TEXCOORD1; // Normal vector in world space
    half fogFactor : TEXCOORD2; // x: fogFactor, yzw: vertex light
    half3 environmentColorAdjustment : TEXCOORD3; // x: fogFactor, yzw: vertex light
    //float3 diff : TEXCOORD2;
    //float2 uv                       : TEXCOORD2; // UVs

    float4 positionCS : SV_POSITION; // Position in clip space
};

// The _MainTex property. The sampler and scale/offset vector is also created
//TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_ST;
// The pyramid height property
//float _PyramidHeight;

// color of the terrain
half3 _TerrainColor;
half _Smoothness;
half _Metallic;

// Vertex functions

VertexOutput Vertex(Attributes input)
{
    // Initialize an output struct
    VertexOutput output = (VertexOutput)0;

    // Use this URP functions to convert position to world space
    // The analogous function for normals is GetVertexNormalInputs
    //VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    //output.positionWS = vertexInput.positionWS;
    
    output.positionWS = TransformObjectToWorld(input.positionOS);

    // TRANSFORM_TEX is a macro which scales and offsets the UVs based on the _MainTex_ST variable
    //output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    return output;
}

// Geometry functions

GeometryOutput SetupVertex(float3 positionWS, half3 normalWS, half3 environmentColorAdjustment/*, float2 uv*/)
{
    // Setup an output struct
    GeometryOutput output = (GeometryOutput)0;
    output.positionWS = positionWS;
    output.normalWS = normalWS;

    // This function calculates clip space position, taking the shadow caster pass into account
    float4 positionCS = CalculatePositionCSWithShadowCasterLogic(positionWS, normalWS);
    //half3 vertexLight = VertexLighting(positionWS, normalWS);
    output.fogFactor = ComputeFogFactor(positionCS.z);

    output.environmentColorAdjustment = environmentColorAdjustment;

    //output.diff = _TerrainColor.rgb * normalWS;


    //output.uv = uv;
    output.positionCS = positionCS;
    return output;
}

// Geometry Shader calculating a normal per triangle; flat lighting
[maxvertexcount(3)]
void Geometry(triangle VertexOutput inputs[3], inout TriangleStream<GeometryOutput> outputStream)
{
    // We need the triangle's normal
    const half3 triNormal = GetNormalFromTriangle(inputs[0].positionWS, inputs[1].positionWS, inputs[2].positionWS);
    const half3 environmentColorAdjustment = SampleSH(triNormal) * _TerrainColor.rgb;

    // Restart the triangle strip, signaling the next appends are disconnected from the last
    //outputStream.RestartStrip();
    // Add the output data to the output stream, creating a triangle
    outputStream.Append(SetupVertex(inputs[0].positionWS, triNormal, environmentColorAdjustment));
    outputStream.Append(SetupVertex(inputs[1].positionWS, triNormal, environmentColorAdjustment));
    outputStream.Append(SetupVertex(inputs[2].positionWS, triNormal, environmentColorAdjustment));
}

// Fragment functions

// The SV_Target semantic tells the compiler that this function outputs the pixel color
float4 Fragment(GeometryOutput input) : SV_Target
{
    #ifdef SHADOW_CASTER_PASS
    // If in the shadow caster pass, we can just return now
    // It's enough to signal that should will cast a shadow
    return 0;
    #else

    //half4 specGloss = half4(0, 0, 0, _Smoothness);
    
    //SurfaceData surfaceData;
    half alpha = 1.0h;
    //surfaceData.alpha = 1;
    //surfaceData.albedo = _TerrainColor.rgb;
    //surfaceData.metallic = _Metallic;
    //surfaceData.specular = half3(0, 0, 0);
    //surfaceData.smoothness = _Smoothness;
    //surfaceData.normalTS = half3(0, 0, 1);
    //surfaceData.occlusion = 1;
    //surfaceData.emission = 0;
    //surfaceData.clearCoatMask = 0;
    //surfaceData.clearCoatSmoothness = 0;
    
    //InputData inputData = (InputData)0;
    //inputData.positionWS = input.positionWS;
    //inputData.normalWS = input.normalWS;
    const float3 viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    //inputData.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    //inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    //inputData.fogCoord = input.fogFactorAndVertexLight.x;
    //inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    //inputData.bakedGI = half3(0, 0, 0);
    //inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    //inputData.shadowMask = half4(1, 1, 1, 1);

    Light mainLight = GetMainLight();
    mainLight.shadowAttenuation = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
    
    // AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
    // mainLight.color *= aoFactor.directAmbientOcclusion;
    // surfaceData.occlusion = min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
    
    BRDFData brdfData;
    // NOTE: can modify alpha
    InitializeBRDFData(_TerrainColor.rgb, _Metallic, half3(0.0h, 0.0h, 0.0h), _Smoothness, alpha, brdfData);
    
    const BRDFData brdfDataClearCoat = (BRDFData)0;

    
    //MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 color = GlobalIllumination(brdfData, brdfDataClearCoat, 0.0h,
                                     half3(0.0h, 0.0h, 0.0h), 1.0h,
                                     input.normalWS, viewDirectionWS);
    color += LightingPhysicallyBased(brdfData, brdfDataClearCoat,
                                     mainLight,
                                     input.normalWS, viewDirectionWS,
                                     0.0h, false);


    const uint pixelLightCount = GetAdditionalLightsCount();
    for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        const Light light = GetAdditionalLight(lightIndex, input.positionWS, half4(1.0h, 1.0h, 1.0h, 1.0h));
        // #if defined(_SCREEN_SPACE_OCCLUSION)
        // light.color *= aoFactor.directAmbientOcclusion;
        // #endif
        color += LightingPhysicallyBased(brdfData, brdfDataClearCoat,
                                         light,
                                         input.normalWS, viewDirectionWS,
                                         0.0h, false);
    }

    //#ifdef _ADDITIONAL_LIGHTS_VERTEX
    //color += inputData.vertexLighting * brdfData.diffuse;
    //#endif

    //color += surfaceData.emission;
    color += input.environmentColorAdjustment;
    color = MixFog(color, input.fogFactor);

    // // MainLight
    // half nl = max(0, dot(input.normalWS, _MainLightPosition.xyz));
    // half4 diff = nl * _MainLightColor;
    //
    // // environment lighting (skybox)
    // diff.rgb += SampleSH(input.normalWS);
    // //diff.rgb = SampleSH(input.normalWS);
    // //diff.a = 1;
    //
    // float4 col = float4(_TerrainColor.rgb, 1);
    // col *= diff;
    
    return half4(color, alpha);;


    // // old super basic mainlight + skylight/environment light lighting
    // InputData lightingInput = (InputData)0;
    // lightingInput.positionWS = input.positionWS;
    // lightingInput.normalWS = input.normalWS; // No need to renormalize, since triangles all share normals
    // lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    // lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS);
    //
    //
    // half4 shadowMask = half4(1, 1, 1, 1);
    // Light mainLight = GetMainLight(lightingInput.shadowCoord, input.positionWS, shadowMask);
    //
    //
    // half nl = max(0, dot(input.normalWS, _MainLightPosition.xyz));
    // half4 diff = nl * _MainLightColor;
    //
    // // environment lighting (skybox)
    // diff.rgb += SampleSH(input.normalWS);
    // //diff.rgb = SampleSH(input.normalWS);
    // //diff.a = 1;
    //
    // float4 col = float4(_TerrainColor.rgb, 1);
    // col *= diff;
    // return col;
    #endif
}

#endif
