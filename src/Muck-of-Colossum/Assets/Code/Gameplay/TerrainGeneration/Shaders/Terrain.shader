Shader "Custom/TerrainURP_CustomLit"
{
    Properties
    {
        baseTextures("Texture Array", 2DArray) = "" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            const static int maxLayerCount = 8;
            
            TEXTURE2D_ARRAY(baseTextures);
            SAMPLER(sampler_baseTextures);

            float minHeight;
            float maxHeight;
            int layerCount;

            float4 baseColours[maxLayerCount];
            float baseStartHeights[maxLayerCount];
            float baseBlends[maxLayerCount];
            float baseColourStrength[maxLayerCount];
            float baseTextureScales[maxLayerCount];

            float inverseLerp(float a, float b, float v)
            {
                return saturate((v - a) / (b - a));
            }

            float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int index)
            {
                float3 uv = worldPos / scale;

                float4 sx = baseTextures.Sample(sampler_baseTextures, float3(uv.y, uv.z, index));
                float4 sy = baseTextures.Sample(sampler_baseTextures, float3(uv.x, uv.z, index));
                float4 sz = baseTextures.Sample(sampler_baseTextures, float3(uv.x, uv.y, index));

                float3 xP = sx.rgb * blendAxes.x;
                float3 yP = sy.rgb * blendAxes.y;
                float3 zP = sz.rgb * blendAxes.z;

                return xP + yP + zP;
            }

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(o.worldPos);
                o.worldNormal = TransformObjectToWorldNormal(v.normalOS);
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);

                float3 blendAxes = abs(normal);
                blendAxes /= (blendAxes.x + blendAxes.y + blendAxes.z + 1e-5);

                float heightPercent = inverseLerp(minHeight, maxHeight, i.worldPos.y);

                float3 albedo = 0;

                for (int layer = 0; layer < layerCount; layer++)
                {
                    float start   = baseStartHeights[layer];
                    float blend   = baseBlends[layer];
                    float strength = baseColourStrength[layer];
                    float scale   = baseTextureScales[layer];

                    float drawStrength = inverseLerp(start - blend * 0.5, start + blend * 0.5, heightPercent);

                    float3 tint = baseColours[layer].rgb * strength;
                    float3 tex  = triplanar(i.worldPos, scale, blendAxes, layer) * (1 - strength);

                    albedo = lerp(albedo, tint + tex, drawStrength);
                }

                // --- охлаждаем общую текстуру ---
                albedo *= float3(0.6, 0.7, 0.9);

                float3 col = 0;

                // ---------- Мягкий основной свет ----------
                Light mainLight = GetMainLight();
                float shadow = MainLightRealtimeShadow(i.positionHCS);
                float NdotL = saturate(dot(normal, mainLight.direction));
                float softLight = NdotL * shadow;

                float3 ambient = albedo * 0.3;

                col += albedo * mainLight.color * softLight + ambient;

                // ---------- Мягкие дополнительные источники ----------
                uint count = GetAdditionalLightsCount();
                for (uint l = 0; l < count; l++)
                {
                    Light light = GetAdditionalLight(l, i.worldPos);
                    float shadow2 = AdditionalLightRealtimeShadow(l, i.positionHCS);
                    float NdotL2 = saturate(dot(normal, light.direction));
                    float softLight2 = NdotL2 * shadow2;

                    col += albedo * light.color * softLight2;
                }

                return float4(col, 1);
            }


            ENDHLSL
        }
    }
    FallBack Off
}
