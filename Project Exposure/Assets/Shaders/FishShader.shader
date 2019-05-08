﻿Shader "Custom/FishShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_WobbleSpeed("WobbleSpeed", Range(0, 1000)) = 3
		_WobbleDistance("WobbleDistance", Range(0, 1)) = 1
		_WobbleCurve("WobbleCurve", Range(0, 1000)) = 5

		_Offset("Offset", Range(0, 1)) = 0
		_FishLength("FishLength", Range(0, 10)) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows addshadow vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float3 vertexPos;
			};


			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			float _WobbleSpeed;
			float _WobbleDistance;
			float _WobbleCurve;

			float _FishLength;
			float _Offset;
			sampler2D _NoiseTex;
			void vert(inout appdata_full v, out Input o)
			{

				//Move uvs
				v.vertex.z += ((sin((_Time + _Offset) * _WobbleSpeed + (-v.vertex.x + _FishLength) * (-v.vertex.x + _FishLength) * _WobbleCurve)) * (-v.vertex.x + _FishLength)) * _WobbleDistance;

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.vertexPos = v.vertex;
			}

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;

				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
