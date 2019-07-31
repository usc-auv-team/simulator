Shader "Simulator/NoiseGround"
{
	Properties
	{
		_Tess("Tessellation", Range(1, 8)) = 4
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_NoiseScale("Noise Scale", float) = 1
		_NoiseFrequency("Noise Frequency", float) = 1
		_NoiseOffset("Noise Offset", Vector) = (0,0,0,0)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows tessellate:tess vertex:vert

			#pragma target 4.6

			#include "NoiseSimplex.cginc" 

			sampler2D _MainTex, _NormalMap;

			struct Input
			{
				float2 uv_MainTex;
			};

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
			};

			float _Tess;
			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			float _NoiseScale;
			float _NoiseFrequency;
			float4 _NoiseOffset;


			float4 tess()
			{
				return _Tess;
			}

			void vert(inout appdata v)
			{
				float3 vertexAxes[3] =
				{
					// vertex position, tangent, and bitangent
					v.vertex.xyz,
					v.vertex.xyz + (v.tangent.xyz * 0.01),
					v.vertex.xyz + (cross(v.normal, v.tangent.xyz) * 0.01)
				};

				for (int i = 0; i < 3; ++i)
				{
					// loop through and apply simplex noise to axes
					float3 afterOffset =
					{
						vertexAxes[i].x + _NoiseOffset.x,
						vertexAxes[i].y + _NoiseOffset.y,
						vertexAxes[i].z + _NoiseOffset.z
					};
					float noise = (_NoiseScale * snoise(afterOffset * _NoiseFrequency) + 1) / 2;
					vertexAxes[i].xyz += noise * v.normal;
				}

				// recalculate the normal as a result of changing the position of the vertex
				float3 updatedNormal = normalize(-1 * cross(vertexAxes[2] - vertexAxes[0], vertexAxes[1] - vertexAxes[0]));

				// write out new changes
				v.vertex.xyz = vertexAxes[0];
				v.normal = updatedNormal;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			}
			ENDCG
		}
			FallBack "Diffuse"
}
