Shader "Custom/DistanceTransparency"
{
    Properties
    {
        _Color("Main Color", Color) = (.5,.5,.5,1)
        _MainTex("Albedo (RGB)", 2D) = "white" { }
        _Cutoff("Alpha Cutoff", Range(0.01, 1.0)) = 0.5
        _DistanceFadeStart("Fade Start Distance", Range(0, 200)) = 10
        _DistanceFadeEnd("Fade End Distance", Range(0, 200)) = 50
        _WorldPosition("World Position", Vector) = (0, 0, 0)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100
            CGPROGRAM
            #pragma surface surf Lambert alpha:blend


            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
                half4 color : COLOR0;
                INTERNAL_DATA
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Cutoff;
            float _DistanceFadeStart;
            float _DistanceFadeEnd;
            float3 _WorldPosition;

            void surf(Input IN, inout SurfaceOutput o)
            {

                float dist = distance(_WorldPosition, IN.worldPos);
                float normalizedDist = saturate((dist - _DistanceFadeStart) / (_DistanceFadeEnd - _DistanceFadeStart));
                float alpha = smoothstep(0, 1, 1 - normalizedDist);

                o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
                o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a * _Color.a * alpha;
            }
            ENDCG
        }
            FallBack "Diffuse"
}