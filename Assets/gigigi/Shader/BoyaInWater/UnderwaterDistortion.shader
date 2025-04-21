Shader "Custom/UnderwaterDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionTex ("Distortion Texture", 2D) = "bump" {}
        _DistortionAmount ("Distortion Amount", Range(0, 0.1)) = 0.02
        
        // 複数の波パラメータ
        _Wave1Speed ("Wave 1 Speed", Range(0, 5)) = 1.2
        _Wave1Frequency ("Wave 1 Frequency", Range(0, 20)) = 8
        _Wave1Amplitude ("Wave 1 Amplitude", Range(0, 0.1)) = 0.01
        
        _Wave2Speed ("Wave 2 Speed", Range(0, 5)) = 0.8
        _Wave2Frequency ("Wave 2 Frequency", Range(0, 20)) = 12
        _Wave2Amplitude ("Wave 2 Amplitude", Range(0, 0.1)) = 0.008
        
        _Wave3Speed ("Wave 3 Speed", Range(0, 5)) = 1.5
        _Wave3Frequency ("Wave 3 Frequency", Range(0, 20)) = 6
        _Wave3Amplitude ("Wave 3 Amplitude", Range(0, 0.1)) = 0.005
        
        _Blur ("Blur Amount", Range(0, 0.05)) = 0.005
        _Transparency ("Transparency", Range(0, 1)) = 0.7
        
        // 水の色設定
        _WaterColor ("Water Color", Color) = (0.0, 0.1, 0.3, 0.5)
        _WaterDepth ("Water Depth Effect", Range(0, 1)) = 0.3
        _WaterFog ("Water Fog Density", Range(0, 1)) = 0.2
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        
        GrabPass { "_GrabTexture" }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float2 distortUV1 : TEXCOORD2;
                float2 distortUV2 : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            sampler2D _DistortionTex;
            float4 _MainTex_ST;
            float4 _DistortionTex_ST;
            float _DistortionAmount;
            
            // 複数の波パラメータ
            float _Wave1Speed, _Wave1Frequency, _Wave1Amplitude;
            float _Wave2Speed, _Wave2Frequency, _Wave2Amplitude;
            float _Wave3Speed, _Wave3Frequency, _Wave3Amplitude;
            
            float _Blur;
            float _Transparency;
            float4 _WaterColor;
            float _WaterDepth;
            float _WaterFog;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                
                // スクリーン座標を取得
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                
                // 異なるスピードと方向で動く2つの歪みテクスチャのUV
                o.distortUV1 = TRANSFORM_TEX(v.uv, _DistortionTex);
                o.distortUV1.x += _Time.y * _Wave1Speed * 0.1;
                o.distortUV1.y += _Time.y * _Wave1Speed * 0.15;
                
                o.distortUV2 = TRANSFORM_TEX(v.uv, _DistortionTex);
                o.distortUV2.x += _Time.y * _Wave2Speed * -0.12;
                o.distortUV2.y += _Time.y * _Wave2Speed * 0.08;
                
                // 視線方向（深度計算用）
                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                
                return o;
            }
            
            // 複数の波を合成する関数
            float2 calculateWaveDistortion(v2f i)
            {
                // 第1の波パターン（横方向）
                float wave1 = sin(i.uv.x * _Wave1Frequency + _Time.y * _Wave1Speed) * _Wave1Amplitude;
                
                // 第2の波パターン（縦方向、異なる周波数）
                float wave2 = sin(i.uv.y * _Wave2Frequency + _Time.y * _Wave2Speed * 0.7) * _Wave2Amplitude;
                
                // 第3の波パターン（対角線方向）
                float wave3 = sin((i.uv.x + i.uv.y) * _Wave3Frequency + _Time.y * _Wave3Speed * 1.3) * _Wave3Amplitude;
                
                // 複数のディストーションテクスチャから取得した値を合成
                float2 distortion1 = UnpackNormal(tex2D(_DistortionTex, i.distortUV1)).rg;
                float2 distortion2 = UnpackNormal(tex2D(_DistortionTex, i.distortUV2)).rg;
                
                // すべての歪みを合成
                float2 finalDistortion;
                finalDistortion.x = distortion1.x * 0.5 + distortion2.x * 0.5 + wave1 + wave3;
                finalDistortion.y = distortion1.y * 0.5 + distortion2.y * 0.5 + wave2 + wave3;
                
                return finalDistortion;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 複合的な波の歪みを計算
                float2 distortion = calculateWaveDistortion(i) * _DistortionAmount;
                
                // 歪みを適用
                float2 grabUV = i.grabPos.xy / i.grabPos.w;
                float2 distortedUV = grabUV + distortion;
                
                // ぼかし効果
                float4 blurColor = 0;
                float blurSize = _Blur;
                float2 blurStep = float2(1.0, 1.0) * blurSize;
                
                // 9点サンプリングでより効率的なぼかし
                blurColor += tex2D(_GrabTexture, distortedUV + float2(-blurStep.x, -blurStep.y)) * 0.0625;
                blurColor += tex2D(_GrabTexture, distortedUV + float2(0, -blurStep.y)) * 0.125;
                blurColor += tex2D(_GrabTexture, distortedUV + float2(blurStep.x, -blurStep.y)) * 0.0625;
                
                blurColor += tex2D(_GrabTexture, distortedUV + float2(-blurStep.x, 0)) * 0.125;
                blurColor += tex2D(_GrabTexture, distortedUV) * 0.25;
                blurColor += tex2D(_GrabTexture, distortedUV + float2(blurStep.x, 0)) * 0.125;
                
                blurColor += tex2D(_GrabTexture, distortedUV + float2(-blurStep.x, blurStep.y)) * 0.0625;
                blurColor += tex2D(_GrabTexture, distortedUV + float2(0, blurStep.y)) * 0.125;
                blurColor += tex2D(_GrabTexture, distortedUV + float2(blurStep.x, blurStep.y)) * 0.0625;
                
                // 背景画像
                float4 grabColor = blurColor;
                
                // 深度に基づく水の色を適用
                float depth = (1.0 - i.viewDir.z) * _WaterDepth;
                float4 waterColor = _WaterColor;
                waterColor.a *= (depth + 0.5); // 深度に基づいて透明度を調整
                
                // 水のフォグ効果 - 距離が遠いほど青みが増す
                float fogFactor = saturate(depth * _WaterFog * 2.0);
                
                // 最終的な色合成
                float4 finalColor = lerp(grabColor, waterColor, fogFactor);
                finalColor.a = max(_Transparency, waterColor.a);
                
                return finalColor;
            }
            ENDCG
        }
    }
}