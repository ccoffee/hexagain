Shader "Custom/DistanceBasedAlphaShader"
{
        Properties
        {
            _Color("Main Color", Color) = (.5, .5, .5, 1)
            _MainTex("Base (RGB)", 2D) = "white" { }
            _OverrideAlpha("Override Alpha", Range(0, 1)) = 1
            _FadeStartDistance("Fade Start Distance", Range(0, 500)) = 5
            _FadeDistance("Fade Distance", Range(0, 50)) = 10
            _FadeSmoothness("Fade Smoothness", Range(0.1, 10.0)) = 1.0
            _Specular("Specular", Color) = (1,1,1,1)
            _Smoothness("Smoothness", Range(0,1)) = 0.5
        }

        SubShader
        {
            Tags { "Queue" = "Overlay" }
            LOD 100

            CGPROGRAM
            #pragma surface surf StandardSpecular alpha
            #pragma target 3.0

            sampler2D _MainTex;
            fixed4 _Color;
            half _OverrideAlpha;
            half _FadeStartDistance;
            half _FadeDistance;
            half _FadeSmoothness;
            float3 _Specular;
            half _Smoothness;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandardSpecular o)
            {
                // Calculate the squared distance from the camera
                float distanceSquared = dot(_WorldSpaceCameraPos - IN.worldPos, _WorldSpaceCameraPos - IN.worldPos);

                // Calculate the fade end distance on the fly
                float fadeEndDistance = _FadeStartDistance + _FadeDistance;

                // Interpolate fade between start and end distances
                float fade = smoothstep(_FadeStartDistance, fadeEndDistance, sqrt(distanceSquared));

                // Apply smoothness adjustment
                fade = pow(fade, _FadeSmoothness);

                // Sample texture and apply color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

                // Apply alpha override and distance-based fading
                c.a = c.a * _OverrideAlpha * fade;

                // Set output color and alpha
                o.Albedo = c.rgb;
                o.Alpha = c.a;
                o.Normal = float3(0, 0, 1);
                o.Specular = _Specular * _Color * c.a;
                o.Smoothness = _Smoothness * c.a;
            }
            ENDCG
        }

        FallBack "Diffuse"
}
