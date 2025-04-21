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
        
        // �����ȃI�u�W�F�N�g�̃����_�����O�ݒ�
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
            // ���̃e�N�X�`���ƃm�C�Y��g�ݍ��킹��
            fixed4 dustTex = tex2D(_DustTexture, IN.uv_DustTexture);
            fixed4 noiseTex = tex2D(_DustNoise, IN.uv_DustTexture * _NoiseScale);
            
            // ���_�J���[���g�p���ăG�b�W�����̚��𔖂�����
            float edgeFactor = 1.0;
            if (IN.color.r > 0) {
                edgeFactor = IN.color.r;
                edgeFactor = pow(edgeFactor, _EdgeFalloff);
            }
            
            // ���̗ʂ𕡐��v�f�Œ���
            float finalDustAmount = _DustAmount * dustTex.r * noiseTex.r * edgeFactor;
            
            // ���̐F�ƃA���t�@��ݒ�
            o.Albedo = _DustColor.rgb * dustTex.rgb;
            o.Alpha = _DustColor.a * finalDustAmount;
            
            // �������������͊��S�ɓ�����
            if (finalDustAmount < 0.05) {
                o.Alpha = 0.0;
            }
            
            // ��{�I��PBR�v���p�e�B
            o.Metallic = 0.0;
            o.Smoothness = 0.2;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}