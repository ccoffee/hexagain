Shader "Custom/HexagonOutlineShader" {
    Properties{
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _Outline("Outline width", Range(0.002, 0.03)) = 0.005
    }

        SubShader{
            Tags {
                "Queue" = "Overlay"
            }
            LOD 100

            Pass {
                Cull Front
                ZWrite On
                ZTest LEqual
                ColorMask RGB

        // Render the backfaces to the stencil buffer
        Stencil {
            Ref 1
            Comp always
            Pass replace
        }

        CGPROGRAM
        #pragma vertex vert
        #pragma exclude_renderers gles xbox360 ps3

        ENDCG
    }

    Pass {
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

        // Only render pixels where the stencil buffer is 0
        Stencil {
            Ref 0
            Comp equal
        }

        CGPROGRAM
        #pragma vertex vert
        #pragma exclude_renderers gles xbox360 ps3

        // The fragment function that handles the outline
        fixed4 frag() : COLOR {
            return _OutlineColor;
        }

        ENDCG
    }

    Pass {
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB

            // Only render pixels where the stencil buffer is 1
            Stencil {
                Ref 1
                Comp equal
            }

            SetTexture[_MainTex] {
                combine primary
            }
        }
    }
}