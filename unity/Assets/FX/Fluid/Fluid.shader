Shader "Fluid/Fluid"
{
	HLSLINCLUDE
		#pragma target 4.5

		#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
		#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
		#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

		Texture2D<float> _DepthTex;
		SamplerState sampler_DepthTex;
		Texture2D _DiffuseTex;
		SamplerState sampler_DiffuseTex;
		float4 _DiffuseTex_TexelSize;
		Texture2D _NormalsTex;
		SamplerState sampler_NormalsTex;
		samplerCUBE _Sky;

		float4 _LightDirection;

		struct vertexInput
		{
			uint vertexID : SV_VertexID;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct vertexOutput
		{
			float4 vertex : SV_POSITION;
			float2 uv   : TEXCOORD0;
			UNITY_VERTEX_OUTPUT_STEREO
		};

		vertexOutput vert(vertexInput input)
		{
			vertexOutput output;
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
			output.vertex = GetFullScreenTriangleVertexPosition(input.vertexID);
			output.uv = GetFullScreenTriangleTexCoord(input.vertexID);
			return output;
		}

		half4 frag_normals(vertexOutput input) : SV_Target
		{
			const float offset = 10.0;
			float2 uv = input.uv;

			float depth = _DepthTex.Sample(sampler_DepthTex, uv);
			if (depth == 0.0)
				discard;
			
			PositionInputs posInput = GetPositionInput(input.vertex.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			float3 P0 = posInput.positionWS;

			uv = float2(input.vertex.x + offset / _ScreenSize.x, input.vertex.y);
			depth = _DepthTex.Sample(sampler_DepthTex, uv * _ScreenSize.zw);
			posInput = GetPositionInput(uv, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			float3 P1 = posInput.positionWS;

			uv = float2(input.vertex.x, input.vertex.y + offset / _ScreenSize.y);
			depth = _DepthTex.Sample(sampler_DepthTex, uv * _ScreenSize.zw);
			posInput = GetPositionInput(uv, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			float3 P2 = posInput.positionWS;

			float3 normal = normalize(cross(P2 - P0, P1 - P0));
			return half4(normal, 1.0);
		}

		half4 frag_blur(vertexOutput input) : SV_Target
		{
			const float offsets[3] = { 0.0, 1.3846153846, 3.2307692308 };
			const float weights[3] = { 0.2270270270, 0.3162162162, 0.0702702703 };
			
			float2 uv = input.uv;

			float depth = _DepthTex.Sample(sampler_DepthTex, uv);
			if (depth == 0.0)
				discard;

			#ifdef VERTICAL
				float2 offset = float2(0.0, _DiffuseTex_TexelSize.y);
			#else
				float2 offset = float2(_DiffuseTex_TexelSize.x, 0.0);
			#endif

			float weight = weights[0];
			float3 result = _DiffuseTex.Sample(sampler_DiffuseTex, uv).rgb * weight;

			offset *= _DiffuseTex_TexelSize.w / 1080.0f;

			[unroll]
			for (int i = 1; i < 3; ++i)
			{
				result += _DiffuseTex.Sample(sampler_DiffuseTex, uv + offset * offsets[i]).rgb * weights[i];
				weight += weights[i];

				result += _DiffuseTex.Sample(sampler_DiffuseTex, uv - offset * offsets[i]).rgb * weights[i];
				weight += weights[i];
			}

			result /= weight + 0.0000001;
			return half4(result, 1.0);
		}

		struct combineOutput
		{
			half4 color : SV_Target;
			float depth : SV_Depth;
		};

		combineOutput frag_compositing(vertexOutput input)
		{
			combineOutput output;

			float2 uv = input.uv;
			float depth = _DepthTex.Load(int3(uv * _DiffuseTex_TexelSize.zw, 0));
			float sourceDepth = LoadCameraDepth(input.vertex.xy);

			if (depth == 0.0 || sourceDepth >= depth)
				discard;

			half4 diffuse = _DiffuseTex.Load(int3(uv * _DiffuseTex_TexelSize.zw, 0));
			half3 normal = _NormalsTex.Load(int3(uv * _DiffuseTex_TexelSize.zw, 0)).rgb;
			normal = normalize(normal);
			

			PositionInputs posInput = GetPositionInput(input.vertex.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			float3 viewDirection = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
			float3 env = texCUBE(_Sky, reflect(viewDirection, normal)).xyz;

			float3 R = reflect(-_LightDirection.xyz, normal);
			float3 specular = float3(1.0, 1.0, 1.0) * pow(saturate(dot(R, viewDirection)), 4.0) * 50.0;

			
			
			output.color = half4((specular + env) * diffuse, 1.0);  //half4(env, 1.0); //diffuse; // half4(normal, 1.0);
			output.depth = depth;
			return output;
		}
	ENDHLSL

	SubShader
	{
		Pass // Pass 0: Compute normals
		{
			ZTest Always
			Cull Off
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag_normals
			ENDHLSL
		}

		Pass // Pass 1: Blur
		{
			ZTest Always
			Cull Off
			ZWrite Off
			HLSLPROGRAM
			#pragma multi_compile __ VERTICAL
			#pragma vertex vert
			#pragma fragment frag_blur
			ENDHLSL
		}

		Pass // Pass 2: Compositing
		{
			ZTest LEqual
			Cull Off
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag_compositing
			ENDHLSL
		}
	}
	FallBack Off
}

