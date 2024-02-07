Shader "Hidden/OverlayShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{
            Cull Off
            ZWrite Off
            ZTest Always
            Tags { "Queue" = "Overlay" }

            Pass {
                Blend SrcAlpha OneMinusSrcAlpha
                ColorMask RGB

                CGPROGRAM
                #pragma vertex vert
                #pragma exclude_renderers gles xbox360 ps3

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 pos : POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;

                v2f vert(appdata v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : COLOR {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col.rgb = 1 - col.rgb; // Invert colors
                    col.a = 0.5;          // Set transparency to 50%
                    return col;
                }
                ENDCG
            }
    }
}