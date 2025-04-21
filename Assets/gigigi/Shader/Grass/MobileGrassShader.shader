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
        
        Cull Off // ���ʕ`��ɕύX�i���ʂ��\���j
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instanced // �C���X�^���V���O�Ή�
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // �C���X�^���XID
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
                float heightFactor : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO // VRChst �p�̃X�e���I�T�|�[�g
            };
            
            float4 _Color;
            float4 _TipColor;
            float _WindSpeed;
            float _WindStrength;
            float _BendAmount;
            float _VariationSeed;
            float4 _PlayerPosition;
            float _PlayerRadius;
            
            // �C���X�^���X�P�ʂ̃v���p�e�B�i�I�v�V�����j
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _InstanceVariation)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            // ���肵���n�b�V���֐�
            float hash(float3 p) {
                p = frac(p * 0.3183099 + _VariationSeed * 0.001);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            
            // ���̗h��̌v�Z
            float3 calculateWind(float3 worldPos, float heightFactor, float instanceVar) {
                // �g��̕�
                float windTime = _Time.y * _WindSpeed;
                float windPhase = worldPos.x * 0.5 + worldPos.z * 0.5;
                float windFactor = sin(windTime + windPhase) * 0.5 + 0.5;
                
                // �C���X�^���X���Ƃ̂΂��
                float variationOffset = instanceVar * 6.28;
                windFactor *= sin(windTime * 0.7 + variationOffset) * 0.5 + 0.5;
                
                // ���̐�[�قǑ傫������
                float heightPower = pow(heightFactor, 2);
                
                // ���̕����iXZ���ʁj
                float3 windDir = float3(sin(windTime * 0.5), 0, cos(windTime * 0.75));
                
                return windDir * windFactor * _WindStrength * heightPower;
            }
            
            // �v���C���[�Ƃ̑��ݍ�p
            float3 calculatePlayerInteraction(float3 worldPos, float heightFactor) {
                float3 toPlayer = worldPos - _PlayerPosition.xyz;
                float dist = length(toPlayer);
                float interaction = 1.0 - saturate(dist / _PlayerRadius);
                
                // �����ɉ������e��
                float heightPower = pow(heightFactor, 2);
                
                // �v���C���[���痣������
                float3 interactionDir = normalize(toPlayer) * interaction * heightPower;
                
                return interactionDir;
            }
            
            v2f vert (appdata v) {
                v2f o;
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                // �C���X�^���X���Ƃ̃o���G�[�V�����l
                float instanceVar = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceVariation);
                if (instanceVar <= 0) {
                    // �C���X�^���X�ϐ����ݒ肳��Ă��Ȃ��ꍇ�A���_�ʒu����n�b�V�����v�Z
                    float3 stablePos = floor(v.vertex.xyz * 100) / 100; // �ʒu���ۂ߂Ĉ��艻
                    instanceVar = hash(stablePos);
                }
                
                // ���̒��_�ʒu
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                // ��������̍����W���̌v�Z�iY�����̑��Έʒu�j
                float heightFactor = saturate(v.vertex.y);
                o.heightFactor = heightFactor;
                
                // ���̋Ȃ���
                float3 bendDir = float3(1, 0, 0);
                float bendAmount = _BendAmount * (1.0 + instanceVar * 0.5); // �C���X�^���X���Ƃ̂΂��
                float3 bend = bendDir * heightFactor * heightFactor * bendAmount;
                
                // ���̉e�����v�Z
                float3 windEffect = calculateWind(worldPos.xyz, heightFactor, instanceVar);
                
                // �v���C���[�Ƃ̑��ݍ�p
                float3 playerEffect = calculatePlayerInteraction(worldPos.xyz, heightFactor);
                
                // �ŏI�I�ȕψʂ�K�p
                float3 displacement = bend + windEffect + playerEffect;
                worldPos.xyz += displacement;
                
                // �N���b�v��Ԃɕϊ�
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                o.worldPos = worldPos.xyz;
                
                // �@���̌v�Z�i���ɂ���]���l���j
                float3 normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                // ���̋����ɉ����Ė@�����X����i�ȈՌv�Z�j
                normal = normalize(normal - displacement * 0.3);
                o.normal = normal;
                
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // �����ɉ������F�̃O���f�[�V����
                fixed4 col = lerp(_Color, _TipColor, i.heightFactor);
                
                // �ȈՓI�ȃ��C�e�B���O
                float3 lightDir = normalize(float3(1, 1, 1));
                float ndotl = max(0.5, dot(normalize(i.normal), lightDir)); // �Œ�Ɩ���ݒ�
                col.rgb *= ndotl;
                
                // �A���t�@�l�𖾎��I�ɐݒ�i�����ɂȂ�Ȃ��悤�ɂ���j
                col.a = 1.0;
                
                return col;
            }
            ENDCG
        }
        
        // �V���h�E�L���X�g�p�̃p�X��ǉ�
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
            
            // �n�b�V���֐��i�ȈՔŁj
            float hash(float3 p) {
                p = frac(p * 0.3183099 + _VariationSeed * 0.001);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            
            v2f vert (appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                // �����W���̌v�Z
                float heightFactor = saturate(v.vertex.y);
                
                // ���̋Ȃ���i�V���h�E�p�Ɋȗ����j
                float3 bendDir = float3(1, 0, 0);
                float3 bend = bendDir * heightFactor * heightFactor * _BendAmount;
                
                // ���[���h���W�ł̒��_�ʒu
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                // ���ɂ��e���i�ȗ����j
                float windTime = _Time.y * _WindSpeed;
                float windFactor = sin(windTime + worldPos.x * 0.5 + worldPos.z * 0.5) * 0.5 + 0.5;
                float3 windDir = float3(sin(windTime * 0.5), 0, cos(windTime * 0.75));
                float3 windEffect = windDir * windFactor * _WindStrength * heightFactor * heightFactor;
                
                // �ψʂ�K�p
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