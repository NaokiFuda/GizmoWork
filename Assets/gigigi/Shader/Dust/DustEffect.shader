Shader "Custom/SimpleDustOverlay" {
    Properties {
        _DustColor ("Dust Color", Color) = (0.5, 0.5, 0.5, 0.8)
        _DustTexture ("Dust Texture", 2D) = "white" {}
        _DustNoise ("Dust Noise", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        _DustAmount ("Dust Amount", Range(0, 1)) = 1.0
        _EdgeFalloff ("Edge Falloff", Range(0, 5)) = 1.0
    }
    
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }
        LOD 200
        
        // 透明なオブジェクトのレンダリング設定
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
        
        struct Input {
            float2 uv_DustTexture;
            float4 color : COLOR;
        };
        
        sampler2D _DustTexture;
        sampler2D _DustNoise;
        half _DustAmount;
        fixed4 _DustColor;
        half _NoiseScale;
        half _EdgeFalloff;
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            // 埃のテクスチャとノイズを組み合わせる
            fixed4 dustTex = tex2D(_DustTexture, IN.uv_DustTexture);
            fixed4 noiseTex = tex2D(_DustNoise, IN.uv_DustTexture * _NoiseScale);
            
            // 頂点カラーを使用してエッジ部分の埃を薄くする
            float edgeFactor = 1.0;
            if (IN.color.r > 0) {
                edgeFactor = IN.color.r;
                edgeFactor = pow(edgeFactor, _EdgeFalloff);
            }
            
            // 埃の量を複数要素で調整
            float finalDustAmount = _DustAmount * dustTex.r * noiseTex.r * edgeFactor;
            
            // 埃の色とアルファを設定
            o.Albedo = _DustColor.rgb * dustTex.rgb;
            o.Alpha = _DustColor.a * finalDustAmount;
            
            // 埃が無い部分は完全に透明に
            if (finalDustAmount < 0.05) {
                o.Alpha = 0.0;
            }
            
            // 基本的なPBRプロパティ
            o.Metallic = 0.0;
            o.Smoothness = 0.2;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}