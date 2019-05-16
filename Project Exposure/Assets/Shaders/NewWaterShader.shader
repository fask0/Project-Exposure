// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewWaterShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_NoiseSpeed("NoiseSpeed", Range(0, 10)) = 1
		_NoiseAmplifier("NoiseAmplifier", Range(0, 1)) = 0.2
		_NoiseFrequency("NoiseFrequency", Range(0, 1)) = 0.1

		_WaveDirection("WaveDirection", Vector) = (0,0,0,0)
		_WaveAmplitude("WaveAmplitude", Range(0, 10)) = 1
		_WaveLength("WaveLength", Range(0, 10)) = 3
		_WaveSpeed("WaveSpeed", Range(0, 100)) = 30
		_WaveSteepness("WaveSteepness", Range(0, 50)) = 0.8
		_WaveColorAmplifier("WaveColorAmplifier", Range(0, 1)) = 0.2

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
		{
			Pass{
			Name "RiverDepth"
			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			Blend SrcAlpha OneMinusSrcAlpha
			Zwrite Off
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma alphatest:_Cutoff
				#pragma addshadows
				#include "UnityCG.cginc"

				struct v2f
				 {
				float4 pos          : POSITION;
				float4 screenPos    : TEXCOORD0;
				float3 vertexPos	: POSITION1;
				};

				v2f vert(appdata_full v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.pos.y -= 4;
					o.screenPos = ComputeScreenPos(o.pos);
					o.vertexPos = v.vertex.xyz;
					return o;
				}

				float4 _Color;
				half4 frag(v2f i) : COLOR
				{

					return float4(_Color.rgb, (sin(i.vertexPos.x) + 1) * 0.5f);
				}
				ENDCG
			}

			Tags { "RenderType" = "Transparent" }
			LOD 200
			Cull Off

			CGPROGRAM
					// Physically based Standard lighting model, and enable shadows on all light types
					#pragma surface surf Standard fullforwardshadows vertex:vert //alpha 

					// Use shader model 3.0 target, to get nicer looking lighting
					#pragma target 5.0
					#include "noiseSimplex.cginc"
					sampler2D _MainTex;

					struct appdata
					{
						float4 vertex : POSITION;
						float4 tangent : TANGENT;
						float3 normal : NORMAL;
						float2 texcoord : TEXCOORD0;
					};

					struct Input
					{
						float2 uv_MainTex;
						float3 vertexPos;
					};

					half _Glossiness;
					half _Metallic;
					fixed4 _Color;

					// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
					// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.

					float4 _WaveDirection;
					float _WaveAmplitude;
					float _WaveLength;
					float _WaveSpeed;
					float _WaveSteepness;
					float _NoiseAmplifier;
					float _NoiseSpeed;
					float _NoiseFrequency;
					void vert(inout appdata v, out Input o)
					{
						for (int i = 0; i < 2; i++)
						{
							float time = _Time;
							if (i > 0)
							{
								time += 3.46f;
							}

							float3 startPos = v.vertex;
							v.vertex.y += snoise((float3(v.vertex.xyz) + (time * _NoiseSpeed)) * _NoiseFrequency) * _NoiseAmplifier;// sin(v.vertex.x + v.vertex.z - (v.vertex.x * v.vertex.z) + (_Time * _NoiseSpeed)) * _NoiseAmplifier;

							float2 dir = float2(_WaveDirection.x, _WaveDirection.z);

							float wi = 2 / _WaveLength;
							float Qi = _WaveSteepness / (_WaveAmplitude * wi * 10);//10);
							float phi = _WaveSpeed * wi;
							float rad = dot(dir, v.vertex.xz) * wi + time * phi;
							v.vertex.y += sin(rad) * _WaveAmplitude;
							v.vertex.xz += cos(rad) * _WaveAmplitude * Qi * dir;

							UNITY_INITIALIZE_OUTPUT(Input, o);
							o.vertexPos = v.vertex * v.normal;
						}
					}

					float _WaveColorAmplifier;
					void surf(Input IN, inout SurfaceOutputStandard o)
					{
						// Albedo comes from a texture tinted by color
						fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

						float colChange = IN.vertexPos.y * _WaveColorAmplifier;
						o.Albedo = c.rgb + float3(colChange, colChange, colChange);
						// Metallic and smoothness come from slider variables
						o.Metallic = _Metallic;
						o.Smoothness = _Glossiness;
						o.Alpha = 0.8f;
					}
					ENDCG
		}
			FallBack "Diffuse"
}