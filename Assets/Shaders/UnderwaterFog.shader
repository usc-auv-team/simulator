Shader "Simulator/UnderwaterFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_FogColor("Fog Color", Color) = (1, 1, 1, 1)
		_DepthStart("Depth Start", float) = 1
		_DepthDistance("Depth Distance", float) = 1

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;
			fixed4 _FogColor;
			float _DepthStart;
			float _DepthDistance;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : COLOR
            {
				float depthValue = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r);
				depthValue *= _ProjectionParams.z;
				depthValue = saturate((depthValue - _DepthStart) / _DepthDistance);
				fixed4 fogColor = _FogColor * depthValue;
				fixed4 col = tex2Dproj(_MainTex, i.screenPos);

				return lerp(col, fogColor, depthValue);
            }
            ENDCG
        }
    }
}
