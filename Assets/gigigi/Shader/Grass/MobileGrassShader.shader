Shader "Custom/MobileGrassShader" {
    Properties {
        _Color ("Base Color", Color) = (0.5, 0.8, 0.2, 1.0)
        _TipColor ("Tip Color", Color) = (0.8, 1.0, 0.4, 1.0)
        _WindSpeed ("Wind Speed", Range(0.0, 10.0)) = 2.0
        _WindStrength ("Wind Strength", Range(0.0, 1.0)) = 0.3
        _BendAmount ("Bend Amount", Range(0.0, 1.0)) = 0.3
        _VariationSeed ("Variation Seed", Range(0, 1000)) = 42
        _PlayerPosition ("Player Position", Vector) = (0, 0, 0, 0)
        _PlayerRadius ("Player Radius", Float) = 2.0
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        
        Cull Off // 両面描画に変更（裏面も表示）
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instanced // インスタンシング対応
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // インスタンスID
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
                float heightFactor : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO // VRChst 用のステレオサポート
            };
            
            float4 _Color;
            float4 _TipColor;
            float _WindSpeed;
            float _WindStrength;
            float _BendAmount;
            float _VariationSeed;
            float4 _PlayerPosition;
            float _PlayerRadius;
            
            // インスタンス単位のプロパティ（オプション）
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _InstanceVariation)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            // 安定したハッシュ関数
            float hash(float3 p) {
                p = frac(p * 0.3183099 + _VariationSeed * 0.001);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            
            // 風の揺れの計算
            float3 calculateWind(float3 worldPos, float heightFactor, float instanceVar) {
                // 波状の風
                float windTime = _Time.y * _WindSpeed;
                float windPhase = worldPos.x * 0.5 + worldPos.z * 0.5;
                float windFactor = sin(windTime + windPhase) * 0.5 + 0.5;
                
                // インスタンスごとのばらつき
                float variationOffset = instanceVar * 6.28;
                windFactor *= sin(windTime * 0.7 + variationOffset) * 0.5 + 0.5;
                
                // 草の先端ほど大きく動く
                float heightPower = pow(heightFactor, 2);
                
                // 風の方向（XZ平面）
                float3 windDir = float3(sin(windTime * 0.5), 0, cos(windTime * 0.75));
                
                return windDir * windFactor * _WindStrength * heightPower;
            }
            
            // プレイヤーとの相互作用
            float3 calculatePlayerInteraction(float3 worldPos, float heightFactor) {
                float3 toPlayer = worldPos - _PlayerPosition.xyz;
                float dist = length(toPlayer);
                float interaction = 1.0 - saturate(dist / _PlayerRadius);
                
                // 高さに応じた影響
                float heightPower = pow(heightFactor, 2);
                
                // プレイヤーから離れる方向
                float3 interactionDir = normalize(toPlayer) * interaction * heightPower;
                
                return interactionDir;
            }
            
            v2f vert (appdata v) {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                // インスタンスごとのバリエーション値
                float instanceVar = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceVariation);
                if (instanceVar <= 0) {
                    // インスタンス変数が設定されていない場合、頂点位置からハッシュを計算
                    float3 stablePos = floor(v.vertex.xyz * 100) / 100; // 位置を丸めて安定化
                    instanceVar = hash(stablePos);
                }
                
                // 元の頂点位置
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                // 根元からの高さ係数の計算（Y方向の相対位置）
                float heightFactor = saturate(v.vertex.y);
                o.heightFactor = heightFactor;
                
                // 草の曲がり
                float3 bendDir = float3(1, 0, 0);
                float bendAmount = _BendAmount * (1.0 + instanceVar * 0.5); // インスタンスごとのばらつき
                float3 bend = bendDir * heightFactor * heightFactor * bendAmount;
                
                // 風の影響を計算
                float3 windEffect = calculateWind(worldPos.xyz, heightFactor, instanceVar);
                
                // プレイヤーとの相互作用
                float3 playerEffect = calculatePlayerInteraction(worldPos.xyz, heightFactor);
                
                // 最終的な変位を適用
                float3 displacement = bend + windEffect + playerEffect;
                worldPos.xyz += displacement;
                
                // クリップ空間に変換
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                o.worldPos = worldPos.xyz;
                
                // 法線の計算（風による回転を考慮）
                float3 normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                // 風の強さに応じて法線を傾ける（簡易計算）
                normal = normalize(normal - displacement * 0.3);
                o.normal = normal;
                
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // 高さに応じた色のグラデーション
                fixed4 col = lerp(_Color, _TipColor, i.heightFactor);
                
                // 簡易的なライティング
                float3 lightDir = normalize(float3(1, 1, 1));
                float ndotl = max(0.5, dot(normalize(i.normal), lightDir)); // 最低照明を設定
                col.rgb *= ndotl;
                
                // アルファ値を明示的に設定（透明にならないようにする）
                col.a = 1.0;
                
                return col;
            }
            ENDCG
        }
        
        // シャドウキャスト用のパスを追加
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f {
                V2F_SHADOW_CASTER;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float _BendAmount;
            float _WindSpeed;
            float _WindStrength;
            float _VariationSeed;
            
            // ハッシュ関数（簡易版）
            float hash(float3 p) {
                p = frac(p * 0.3183099 + _VariationSeed * 0.001);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            
            v2f vert (appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                // 高さ係数の計算
                float heightFactor = saturate(v.vertex.y);
                
                // 草の曲がり（シャドウ用に簡略化）
                float3 bendDir = float3(1, 0, 0);
                float3 bend = bendDir * heightFactor * heightFactor * _BendAmount;
                
                // ワールド座標での頂点位置
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                // 風による影響（簡略化）
                float windTime = _Time.y * _WindSpeed;
                float windFactor = sin(windTime + worldPos.x * 0.5 + worldPos.z * 0.5) * 0.5 + 0.5;
                float3 windDir = float3(sin(windTime * 0.5), 0, cos(windTime * 0.75));
                float3 windEffect = windDir * windFactor * _WindStrength * heightFactor * heightFactor;
                
                // 変位を適用
                worldPos.xyz += bend + windEffect;
                v.vertex = mul(unity_WorldToObject, worldPos);
                
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            float4 frag (v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}