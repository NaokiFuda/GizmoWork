Shader "Custom/ReliableGrassShader" {
    Properties {
        _Color ("Color", Color) = (0.5, 0.8, 0.2, 1.0)
        _TipColor ("Tip Color", Color) = (0.8, 1.0, 0.4, 1.0)
        _GrassHeight ("Grass Height", Range(0.1, 5.0)) = 1.0
        _GrassWidth ("Grass Width", Range(0.02, 0.5)) = 0.1
        _RandomHeight ("Height Randomness", Range(0.0, 1.0)) = 0.25
        _WindSpeed ("Wind Speed", Range(0.0, 10.0)) = 2.0
        _WindStrength ("Wind Strength", Range(0.0, 1.0)) = 0.3
        _InteractorPosition ("Interactor Position", Vector) = (0, 0, 0, 0)
        _InteractorRadius ("Interaction Radius", Float) = 2.0
        _InteractionStrength ("Interaction Strength", Range(0.0, 5.0)) = 2.0
        _Density ("Grass Density", Range(1, 10)) = 3
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _BladeCurve ("Blade Curvature", Range(0.0, 3.0)) = 1.0
        _BladeSegments ("Blade Segments", Range(3, 8)) = 5
        _NodeAmount ("Node Amount", Range(0.0, 1.0)) = 0.3
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        Cull Off // ���ʃ����_�����O
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2g {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct g2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float heightFactor : TEXCOORD2;
                float3 normal : NORMAL;
            };
            
            float4 _Color;
            float4 _TipColor;
            float _GrassHeight;
            float _GrassWidth;
            float _RandomHeight;
            float _WindSpeed;
            float _WindStrength;
            float4 _InteractorPosition;
            float _InteractorRadius;
            float _InteractionStrength;
            int _Density;
            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;
            float _BladeCurve;
            int _BladeSegments;
            float _NodeAmount;
            
            // �����_���l�����֐�
            float rand(float3 seed) {
                return frac(sin(dot(seed.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }
            
            // �p�[�����m�C�Y�̊ȈՎ���
            float simpleNoise(float2 uv) {
                return tex2Dlod(_NoiseTexture, float4(uv, 0, 0)).r;
            }
            float hash(float3 p) {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }
            
            // �x�W�F�Ȑ��ɂ�鑐�̋Ȃ���v�Z
            float3 bezier(float3 p0, float3 p1, float3 p2, float t) {
                float oneMinusT = 1.0 - t;
                return oneMinusT * oneMinusT * p0 + 2.0 * oneMinusT * t * p1 + t * t * p2;
            }
            
            v2g vert (appdata v) {
                v2g o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.uv = v.uv;
                return o;
            }
            
            // ���_�𐶐�����֐�
            g2f CreateVertex(float3 pos, float2 uv, float3 worldPos, float heightFactor, float3 normal) {
                g2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv;
                o.worldPos = worldPos;
                o.heightFactor = heightFactor;
                o.normal = normal;
                return o;
            }
            
            [maxvertexcount(64)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream) {
                // �O�p�`�̒��S�_���v�Z
                float3 centerPos = (input[0].vertex.xyz + input[1].vertex.xyz + input[2].vertex.xyz) / 3.0;
                float3 centerNormal = normalize((input[0].normal + input[1].normal + input[2].normal) / 3.0);
                
                // �ʐςɊ�Â��đ��̖��x�𒲐�
                float3 edge1 = input[1].vertex.xyz - input[0].vertex.xyz;
                float3 edge2 = input[2].vertex.xyz - input[0].vertex.xyz;
                float area = length(cross(edge1, edge2)) / 2.0;
                int grassCount = clamp(int(area * _Density * 10), 1, 5);
                
                // �Z�O�����g���ݒ�
                int segments = clamp(_BladeSegments, 3, 5);
                
                // �e���̐���
                for (int i = 0; i < grassCount; i++) {
                    // �����_���ȑ��̈ʒu���O�p�`���ɐ���
                    float r1 = rand(centerPos + float3(i, i*2, i*3));
                    float r2 = rand(centerPos + float3(i*4, i*5, i*6));
                    
                    // �d�S���W���g���ĎO�p�`���̃����_���ȓ_���v�Z
                    float sqrtR1 = sqrt(r1);
                    float u = 1.0 - sqrtR1;
                    float v = r2 * sqrtR1;
                    float w = 1.0 - u - v;
                    
                    float3 grassPos = input[0].vertex.xyz * u + input[1].vertex.xyz * v + input[2].vertex.xyz * w;
                    float3 stablePos = floor(grassPos * 100) / 100;
                    float3 grassNormal = normalize(input[0].normal * u + input[1].normal * v + input[2].normal * w);
                    
                    // �����_���ȑ��̍����ƌ���������
                    float heightRandom = 1.0 + _RandomHeight * (hash(stablePos) * 2.0 - 1.0);
                    float grassHeight = _GrassHeight * heightRandom;
                    
                    // ���̃u���[�h�̌����������_����
                    float randomAngle = rand(grassPos + float3(1.0, 2.0, 3.0)) * 3.14159 * 2.0;
                    float3 grassTangent = normalize(cross(grassNormal, float3(sin(randomAngle), 0, cos(randomAngle))));
                    float3 grassBitangent = normalize(cross(grassNormal, grassTangent));
                    
                    // ���[���h���W�ɕϊ�
                    float3 worldGrassPos = mul(unity_ObjectToWorld, float4(grassPos, 1.0)).xyz;
                    
                    // ���̌��ʁi���ԂɊ�Â��h��j
                    float windNoise = simpleNoise(worldGrassPos.xz * 0.1 + _Time.y * _WindSpeed * 0.1);
                    float windFactor = sin(_Time.y * _WindSpeed + worldGrassPos.x * 0.5 + worldGrassPos.z * 0.5) * windNoise;
                    
                    // �C���^���N�V�����̌���
                    float3 toInteractor = _InteractorPosition.xyz - worldGrassPos;
                    float distanceToInteractor = length(toInteractor);
                    float interactionFactor = 1.0 - saturate(distanceToInteractor / _InteractorRadius);
                    float3 interactionDir = normalize(toInteractor) * interactionFactor * _InteractionStrength;
                    
                    // ���̍����̈ʒu
                    float3 basePos = grassPos;
                    
                    // ���̐�[�̈ʒu
                    float3 tipPos = basePos + grassNormal * grassHeight;
                    
                    // �x�W�F�Ȑ��̃R���g���[���|�C���g�v�Z
                    // ���ƃC���^���N�V�����̉e�����󂯂�R���g���[���|�C���g
                    float3 windEffect = (grassTangent * windFactor + grassBitangent * windFactor * 0.3) * _WindStrength;
                    float3 controlPoint = lerp(basePos, tipPos, 0.5) + 
                        windEffect * 0.5 * _BladeCurve - 
                        interactionDir * 0.5;
                    
                    // ��[�ւ̕��ƃC���^���N�V�����̉e��
                    float3 adjustedTipPos = tipPos + windEffect - interactionDir;
                    
                    float bladeWidth = _GrassWidth;
                    
                    // �e�Z�O�����g���Ƃɒ��ڎO�p�`�X�g���b�v���o��
                    for (int j = 0; j < segments; j++) {                        
                        float t0 = float(j) / float(segments);
                        float t1 = float(j + 1) / float(segments);
                        
                        // ���݂̃Z�O�����g�̒��_�ʒu���v�Z
                        float3 segmentPos0 = bezier(basePos, controlPoint, adjustedTipPos, t0);
                        float3 segmentPos1 = bezier(basePos, controlPoint, adjustedTipPos, t1);
                        
                        // �߂̌���
                        float nodeEffect0 = sin(t0 * 3.14159 * 2.0 * 2.0) * 0.5 + 0.5;
                        nodeEffect0 = pow(nodeEffect0, 3) * _NodeAmount;
                        
                        float nodeEffect1 = sin(t1 * 3.14159 * 2.0 * 2.0) * 0.5 + 0.5;
                        nodeEffect1 = pow(nodeEffect1, 3) * _NodeAmount;
                        
                        // ���̌v�Z�i��[�Ɍ������čׂ��Ȃ� + �߂̌��ʁj
                        float width0 = bladeWidth * pow(1.0 - t0, 0.3) * (1.0 + nodeEffect0 * 0.2);
                        float width1 = bladeWidth * pow(1.0 - t1, 0.3) * (1.0 + nodeEffect1 * 0.2);
                        
                        // �@���v�Z
                        float3 forward0 = normalize(segmentPos1 - segmentPos0);
                        float3 side0 = normalize(cross(forward0, grassTangent));
                        
                        // �e���_�̖@��
                        float3 normalLeft = normalize(cross(side0, forward0));
                        float3 normalRight = normalize(cross(forward0, side0));
                        
                        // ����
                        float3 posLL = segmentPos0 - side0 * width0 * 0.5;
                        float3 worldPosLL = mul(unity_ObjectToWorld, float4(posLL, 1.0)).xyz;
                        g2f vertLL = CreateVertex(posLL, float2(0, t0), worldPosLL, t0, normalLeft);
                        
                        // �E��
                        float3 posLR = segmentPos0 + side0 * width0 * 0.5;
                        float3 worldPosLR = mul(unity_ObjectToWorld, float4(posLR, 1.0)).xyz;
                        g2f vertLR = CreateVertex(posLR, float2(1, t0), worldPosLR, t0, normalRight);
                        
                        // ����
                        float3 posUL = segmentPos1 - side0 * width1 * 0.5;
                        float3 worldPosUL = mul(unity_ObjectToWorld, float4(posUL, 1.0)).xyz;
                        g2f vertUL = CreateVertex(posUL, float2(0, t1), worldPosUL, t1, normalLeft);
                        
                        // �E��
                        float3 posUR = segmentPos1 + side0 * width1 * 0.5;
                        float3 worldPosUR = mul(unity_ObjectToWorld, float4(posUR, 1.0)).xyz;
                        g2f vertUR = CreateVertex(posUR, float2(1, t1), worldPosUR, t1, normalRight);
                        
                        // �O�p�`�X�g���b�v�ŏo��
                        triStream.Append(vertLL);
                        triStream.Append(vertLR);
                        triStream.Append(vertUL);
                        triStream.Append(vertUR);
                        triStream.RestartStrip();
                    }
                }
            }
            
            fixed4 frag (g2f i) : SV_Target {
                // ���������[�ɂ����ĐF���O���f�[�V����
                fixed4 col = lerp(_Color, _TipColor, i.heightFactor);
                
                // �߂̌���
                float nodeEffect = sin(i.heightFactor * 3.14159 * 2.0 * 2.0) * 0.5 + 0.5;
                nodeEffect = pow(nodeEffect, 3) * _NodeAmount;
                
                // �߂̕����������Â�����
                col.rgb *= 1.0 - nodeEffect * 0.2;
                
                // �ȈՓI�Ȍ������
                float gloss = pow(i.heightFactor, 3) * 0.3;
                col.rgb += gloss;
                
                // �ȈՓI�ȃ��C�e�B���O
                float3 lightDir = normalize(float3(1, 1, 1));
                float ndotl = max(0, dot(i.normal, lightDir));
                col.rgb *= lerp(0.7, 1.0, ndotl);
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}