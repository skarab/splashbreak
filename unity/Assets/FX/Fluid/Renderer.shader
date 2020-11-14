Shader "Fluid/Renderer"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("MainTex (RGB) ", 2D) = "" {}
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        Pass
        {
            ZTest LEqual
            Cull Off
            ZWrite On
            Blend Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            half4 _Color;

            struct VertexInput
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 color : TEXCOORD1;
            };
            
            struct FragmentOutput
            {
                float depth : COLOR0;
                half4 diffuse : COLOR1;
            };

            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;
                output.position = TransformObjectToHClip(input.position.xyz);
                output.uv = input.uv;
                output.color = input.color.rgb;
                return output;
            }
          
            FragmentOutput frag(VertexOutput input)
            {
                const float alphaThreshold = 0.25;
                const float depthStrength = 0.002;

                half4 color = _MainTex.Sample(sampler_MainTex, input.uv);
                if (color.a < alphaThreshold)
                    discard;

                FragmentOutput output;
                output.depth = input.position.z + color.a * depthStrength;
                output.diffuse = half4(input.color, 1.0) * _Color;
                return output;
            }
            ENDHLSL
        }
    }
}
