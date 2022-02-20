Shader "Custom/TerrainShader"
{
    Properties
    {
        // shader properties; editable in material
        //_MainTex ("Texture", 2D) = "white" {}
        _TerrainColor ("Terrain Color", Color) = (.25, .5, .3, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        //LOD 100

        Pass
        {

            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Cull Back

            HLSLPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            //#pragma surface surf Standard fullforwardshadows
            
            // Signal this shader requires geometry programs
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma require geometry

            //
            //#pragma multi_compile_shadowcaster

            // Lighting and shadow keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            
            //#pragma multi_compile _ _SPECGLOSSMAP
            //#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

            // Register our functions
            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment

            //#define SHADOW_CASTER_PASS

            // Include our logic file
            #include "TerrainShader.hlsl"
            ENDHLSL
        }

        // Shadow caster pass. This pass renders a shadow map.
        // We treat it almost the same, except strip out any color/lighting logic
        Pass
        {

            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            HLSLPROGRAM
            // Signal this shader requires geometry programs
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma require geometry

            // This sets up various keywords for different light types and shadow settings
            #pragma multi_compile_shadowcaster

            // Register our functions
            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment

            // Define a special keyword so our logic can change if inside the shadow caster pass
            #define SHADOW_CASTER_PASS

            // Include our logic file
            #include "TerrainShader.hlsl"
            ENDHLSL
        }
    }
}