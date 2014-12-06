Shader "Custom/AlwaysVisibleUnlitWorldSpace" 
{
	Properties 
	{
		_MainTint ("Color Tint(RGB)", Color) = (1, 1, 1, 1)
		_MainTex ("Base Texture(RGBA)", 2D) = "white"{}
		
		_OverlayTint ("Overlay Tint(RGB)", Color) = (1, 1, 1, 1)
		_OverlayTex ("Overlay Texture(RGBA)", 2D) = "white"{}
		
		_OverlayAlpha ("Overlay Alpha", Float) = 0.5
		
		_TexScale("Texture Scale", Vector) = (1, 1, 1, 1)
	}
	SubShader 
	{
		Tags { "RenderType" = "Transparent" "Queue"="Transparent"}
		LOD 200
		
		Ztest Less
		Zwrite On
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Unlit
		#include "UnityCG.cginc"
		
		fixed4 LightingUnlit(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		sampler2D _OverlayTex;
		float4 _MainTint;
		float4 _OverlayTint;
		float _OverlayAlpha;
		float4 _TexScale;

		struct Input 
		{
			float4 worldPos; 
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float4 null = float4(0.0, 0.0, 0.0, 0.0);
		
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
          	screenUV *= _TexScale.xy;
          	
			float4 t = tex2D(_MainTex, screenUV);
			float4 ot = lerp(null, tex2D(_OverlayTex, IN.uv_MainTex), _OverlayAlpha);
		
			o.Albedo = lerp(t.rgb * _MainTint.rgb, ot.rgb * _OverlayTint.rgb, ot.a);
			o.Alpha = t.a * _MainTint.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
