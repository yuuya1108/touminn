
Shader "Custom/SpriteRedOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _Thickness ("Outline Thickness", Float) = 4
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _Thickness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 offset = _MainTex_TexelSize.xy * _Thickness;

                half centerAlpha = SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, input.uv
                ).a;

                half surroundingAlpha = 0;

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(offset.x, 0)).a);

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - float2(offset.x, 0)).a);

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, offset.y)).a);

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - float2(0, offset.y)).a);

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + offset).a);

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - offset).a);

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(offset.x, -offset.y)).a);

                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(-offset.x, offset.y)).a);

                half outlineAlpha = surroundingAlpha * (1 - centerAlpha);

                return half4(
                    _OutlineColor.rgb,
                    outlineAlpha * _OutlineColor.a * input.color.a
                );
            }

            ENDHLSL
        }
    }
}
