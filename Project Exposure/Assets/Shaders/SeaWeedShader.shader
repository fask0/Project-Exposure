Shader "Custom/SeaWeedShader"
{
	Properties
	{
		_MinDist("Min Tessellation Distance", float) = 10
		_MaxDist("Max Tessellation Distance", float) = 50
		_Tess("Tessellation", Range(1, 20)) = 4
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_WobbleSpeed("WobbleSpeed", Range(0, 1)) = 0.5
		_WobbleDistance("WobbleDistance", Range(0, 1)) = 0.5
		_WobbleCurve("WobbleCurve", Range(0, 1)) = 0.5

		_LeanDirection("LeanDirection", Vector) = (1, 0,0,0)
		_LeanDistance("LeanDistance", Range(0, 10)) = 4
		_Offset("Offset", float) = 0

		_HighestY("HighestY", float) = 1
		_LowestY("LowestY", float) = 0

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
		{
			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			/*Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }*/
			Cull Off

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tess /*alpha:fade*/  alphatest:_Cutoff addshadow

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 5.0
			#include "noiseSimplex.cginc"
			#include "Tessellation.cginc"

			sampler2D _MainTex;
			sampler2D _NormalMap;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
			};

			struct Input
			{
				float2 uv_MainTex;
				float3 vertexPos;
			};


			float _MinDist;
			float _MaxDist;
			float _Tess;
			float4 tess(appdata v0, appdata v1, appdata v2)
			{
				return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _MinDist, _MaxDist, _Tess);
			}

			float _WobbleSpeed;
			float _WobbleDistance;
			float _WobbleCurve;

			float2 _LeanDirection;
			float _LeanDistance;
			float _HighestY;
			float _LowestY;
			float _Offset;
			sampler2D _NoiseTex;
			void vert(inout appdata v)
			{
				//Vertex pos to world pos shite
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				float diff = _HighestY - _LowestY;
				float relativeYPos = ((wPos.y - _LowestY) / diff);

				float4 v0 = wPos;
				//The lean bois
				v0.x += (relativeYPos * relativeYPos * _LeanDistance) * _LeanDirection.x;
				v0.z += (relativeYPos * relativeYPos * _LeanDistance) * _LeanDirection.y;

				//The wobble wobble wobble
				v0.x += ((snoise((_Time + _Offset) * _WobbleSpeed + relativeYPos * relativeYPos * _WobbleCurve)) * relativeYPos) * _WobbleDistance;
				v0.z += ((snoise((_Time + _Offset / 2) * _WobbleSpeed + relativeYPos * relativeYPos * _WobbleCurve) + 0.5) * relativeYPos) * _WobbleDistance;

				float4 test = mul(unity_WorldToObject, v0);
				v.vertex.xyz = test;
			}

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
				o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			}
			ENDCG
		}
			FallBack "Diffuse"
}
