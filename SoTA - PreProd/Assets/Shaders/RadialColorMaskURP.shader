Shader "Unlit/RadialColorMaskURP"
{
    /// <summary>
    /// Author:Karin
    /// 
    /// Modified by:
    /// 
    /// </summary>
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1) // The _ is needed for shaders, I'm not braking our coding conventions :'D
        _MainTex ("Texture", 2D) = "white" {}
        _EffectRadius ("Effect Radius", Float) = 0.2 // Radius in UV space (0 to 1)
        _EffectRadiusSmoothing ("Effect Radius Smoothing", Float) = 0.02 // Smooth edge area in UV space
        _EnableEffect ("Enable Effect", Float) = 1 // This will allow toggling the shader on and off basically
        _HealthBlackout ("Health Blackout", Range(0,1)) = 0 // For turning the screen black when away from star
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #define MAX_LIGHT_SOURCE_NUM 10 // Needs to be known at compile time

            float _EffectRadius;
            float _EffectRadiusSmoothing;
            float4 _StarPosition;
            int _ActiveLightCount;
            float4 _LightPositions[MAX_LIGHT_SOURCE_NUM]; 
            float2 _ScreenResolution;
            float _EnableEffect;
            float _HealthBlackout;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f // v2f = vertex to fragment
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float GetMask(float4 lightPosition, float2 uv)
            {
                float2 center = lightPosition.xy;
                float2 uvNormalized = uv - center;

                // Adjust for aspect ratio to keep effect circular on all screen sizes
                float aspect = _ScreenResolution.y / _ScreenResolution.x;               
                uvNormalized.y *= aspect;

                float dist = length(uvNormalized); // Distance in normalized UV space (0 to 1)

                // Smoothstep creates a smooth transition from 0 to 1
                return smoothstep(_EffectRadius - _EffectRadiusSmoothing, _EffectRadius + _EffectRadiusSmoothing, dist);
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv); // Samples from texture (the frame buffer in this case)

                // Apply the mask using the star's position
                float mask = GetMask(_StarPosition, uv);

                // Apply additional masks from each light
                for (int i = 0; i < _ActiveLightCount; ++i)
                {
                    mask *= GetMask(_LightPositions[i], uv);
                }

                mask *= _EnableEffect; // If _EnableEffect is 1, shader gets applied, if 0 it does not

                // Convert to greyscale
                half grayscale = dot(col.rgb, half3(0.1, 0.3, 0.05)); // Intensity of the grey
                half4 greyCol = half4(grayscale, grayscale, grayscale, 1);
                greyCol = lerp(greyCol, half4(0,0,0,1), _HealthBlackout);

                return lerp(col, greyCol, mask); // Mask is either 0 (color) or 1 (greyscale)
            }
            ENDHLSL
        }
    }
}