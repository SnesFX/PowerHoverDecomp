Shader "Custom/AltitudeFog" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FogColor ("Fog Color", Vector) = (0.5,0.5,0.5,1)
		_FogMaxHeight ("Fog Max Height", Float) = 0
		_FogMinHeight ("Fog Min Height", Float) = -1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}