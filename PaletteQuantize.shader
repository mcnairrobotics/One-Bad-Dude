Shader "Hidden/URP/PaletteQuantize"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Colors ("Color Steps", Range(2, 64)) = 16
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Colors;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // Palette quantization
                col.rgb = floor(col.rgb * _Colors) / _Colors;
                float luma = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = lerp(float3(luma, luma, luma), col.rgb, 0.9);

                return col;
            }
            ENDHLSL
        }
    }
}
