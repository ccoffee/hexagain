Shader "Custom/VertexColorTransparent"
{
    Properties
    {
        _Color("Main Color", Color) = (.5, .5, .5, 1)
        _MainTex("Albedo (RGB)", 2D) = "white" { }
        _AlphaOverride("Alpha Override", Range(0.0, 1.0)) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ColorMask RGB

            CGPROGRAM
            #pragma surface surf Standard alpha:fade

            struct Input
            {
                float2 uv_MainTex;
                half4 color : COLOR;
                INTERNAL_DATA
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _AlphaOverride;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);

                // Combine vertex color and texture color
                o.Albedo = texColor.rgb * IN.color.rgb;

                // Set alpha using the alpha channel of the texture, multiplied by the alpha override
                o.Alpha = texColor.a * _AlphaOverride;
            }
            ENDCG
        }

            // Fallback for other render types
                FallBack "Diffuse"
}