Shader "Custom/CustomLight" {
	Properties {
		_DiffuseExponent ("Diffuse Exponent", Float) = 5
		_Glossiness ("Glossiness", Range(0, 1)) = 0
		_Color ("Color", Vector) = (1,1,1,1)
		_SpecularColor ("Specular Color", Vector) = (0.5,0.5,0.5,1)
		_Normal ("Normal", 2D) = "bump" {}
		_NormalStrength ("Normal Strength", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	Fallback "VertexLit"
}