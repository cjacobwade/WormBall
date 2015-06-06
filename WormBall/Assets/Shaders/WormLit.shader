Shader "Custom/WormLit" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_MainTint ("Color Tint(RGB)", Color) = (1, 1, 1, 1)
		
		_OverlayTint ("Overlay Tint(RGB)", Color) = (1, 1, 1, 1)
		_OverlayTex ("Overlay Texture(RGBA)", 2D) = "white"{}
		
		_OverlayAlpha ("Overlay Alpha", Float) = 0.5
		
		_TexScale("Texture Scale", Vector) = (1, 1, 1, 1)
	}
	SubShader 
	{
		//Tags { "RenderType" = "Transparent" "Queue"="Transparent"}
		//LOD 200
		
		//Ztest Less
		//Zwrite On
		//Blend SrcAlpha OneMinusSrcAlpha
		
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#include "UnityCG.cginc"
		
		#pragma target 3.0
		
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
		
		float _Metallic;
		float _Glossiness;

		struct Input 
		{
			float4 worldPos; 
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float4 null = float4(0.0, 0.0, 0.0, 0.0);
		
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
          	screenUV *= float2(8, 5);
          	
			float4 t = tex2D(_MainTex, screenUV);
			float4 ot = lerp(null, tex2D(_OverlayTex, IN.uv_MainTex), _OverlayAlpha);
		
			o.Albedo = lerp(t.rgb * _MainTint.rgb, ot.rgb * _OverlayTint.rgb, ot.a);
			o.Alpha = t.a * _MainTint.a;
			
			o.Smoothness = _Glossiness;
			o.Metallic = _Metallic;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
