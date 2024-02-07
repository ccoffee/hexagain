Shader "Custom/ShittyShader"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" { }
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            CGPROGRAM
            #pragma surface surf Lambert decal:blend

            struct Input
            {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;

            void surf(Input IN, inout SurfaceOutput o)
            {

                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
                o.Alpha = 0.5;
            }
            ENDCG
        }
}