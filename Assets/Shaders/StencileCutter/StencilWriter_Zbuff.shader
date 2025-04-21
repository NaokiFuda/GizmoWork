Shader "Stenciles/StencilWriter_Zbuff_Stable_Fixed"
{
    Properties
    {
        _StencilID ("Stencil ID", Range(0, 255)) = 1
        _HitPosition("Hit Position", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Geometry+3" "RenderType"="Transparent" }

        Pass
        {
            Name "VisibleOnlyStencilPass"


            Stencil
            {
                Ref [_StencilID]  
                WriteMask [_StencilID]
                Comp Always
                Pass Replace
            }
            
            ZWrite Off
            ZTest LEqual
            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


             float4 _HitPosition;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_HitPosition.w < 0.5)

                    discard;

                return 0;
            }
            ENDCG
        }

    }
}
