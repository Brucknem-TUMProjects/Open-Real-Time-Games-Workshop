Shader "Unlit/UnfoLight"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_LightLength("Light Length", Float) = 1
		_LightWorldPos("Light World Pos", Vector) = (0,0,0,0)
		_Noise ("Perlin Noise", 2D) = "defaulttexture" {}
		_NoiseSpeed ("Noise Speed", Float) = 0.5
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma target 3.5
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 distance : POSITION1;
			};

			float4 _Color;
			float4 _LightWorldPos;
			sampler2D _Noise;
			float _NoiseSpeed;
			float _LightLength;
			
			v2f vert (appdata v, uint vid : SV_VertexID)
			{
				v2f o;
				o.distance = mul(unity_ObjectToWorld, v.vertex) - _LightWorldPos;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Color;
				float distance = length(i.distance.xyz);
				float intensity = 3 * clamp(_LightLength / distance, 0, 1);
				float2 uv = float2((float)(i.uv.x + _Time * _NoiseSpeed), (float)(i.uv.y + _Time * _NoiseSpeed));
				fixed4 noise = tex2D(_Noise, uv);
				col = intensity * (0.5f * col + 0.5f * col * pow(noise, 2));
				col = float4(clamp(col.r, 0, 1), clamp(col.g, 0, 1), clamp(col.b, 0, 1), clamp(col.a, 0, 1));
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
