Shader "Stenciles/ZWriteOnly"
{
    Properties
    {
        _HitPosition ("Hit Position", Vector) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        ZWrite On
        ColorMask 0
        ZTest LEqual
        Cull Back
        Lighting Off

        Pass
        {
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
                return 0; // ColorMask 0 ‚É‚æ‚è•`‰æ‚³‚ê‚È‚¢
            }
            ENDCG
        }
    }
    FallBack Off
}
