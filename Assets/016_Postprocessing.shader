Shader "Tutorial/016_Postprocessing" {
    Properties{
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{
            Cull Off
            ZWrite Off
            ZTest Always

            Tags {"Queue" = "Overlay" }

            Pass {
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #include "UnityCG.cginc"

                #pragma vertex vert
                #pragma fragment frag

                sampler2D _MainTex;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.position = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_TARGET {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    col = 1 - col;

                    // Apply 50% transparency
                    col.a = 0.5;

                    return col;
                }
                ENDCG
            }
    }
}