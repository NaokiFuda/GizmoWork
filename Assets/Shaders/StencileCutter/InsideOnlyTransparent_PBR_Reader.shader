Shader "Stenciles/InsideOnlyTransparent_PBR_Reader"
{
    Properties
    {
        _MainTex ("Base Color", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Metallic ("Metallic", Range(0,1)) = 0
        _MetallicMap ("Metallic", 2D) = "black" {}

        _Roughness ("Roughness", Range(0,1)) = 0.5
        _RoughnessMap ("Roughness", 2D) = "white" {}

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _Emission ("Emission", Range(0,10)) = 0

        _StencilID("Stencil ID", Float) = 1
        _HitPosition("Hit Position", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry+5" }
        LOD 300


        // パス1: ステンシル以外の通常描画
        Pass
        {
            Name "OutsideStencil"
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            ZTest LEqual

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityStandardUtils.cginc"
            #include "UnityPBSLighting.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MetallicMap;
            float4 _MetallicMap_ST;
            sampler2D _RoughnessMap;
            float4 _RoughnessMap_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            sampler2D _EmissionMap;
            float4 _EmissionMap_ST;

            float4 _Color;
            float _Metallic;
            float _Roughness;
            float _Emission;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w;
                o.worldPos = worldPos;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.uv = v.uv;
                o.tangentDir = worldTangent;
                o.bitangentDir = worldBitangent;
                o.normalDir = worldNormal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uvMain = TRANSFORM_TEX(i.uv, _MainTex);
                float2 uvMetallic = TRANSFORM_TEX(i.uv, _MetallicMap);
                float2 uvRoughness = TRANSFORM_TEX(i.uv, _RoughnessMap);
                float2 uvNormal = TRANSFORM_TEX(i.uv, _NormalMap);
                float2 uvEmission = TRANSFORM_TEX(i.uv, _EmissionMap);

                float3x3 TBN = float3x3(normalize(i.tangentDir), normalize(i.bitangentDir), normalize(i.normalDir));
                float3 normalMap = tex2D(_NormalMap, uvNormal).xyz * 2 - 1;
                float3 N = normalize(mul(normalMap, TBN));
                float3 V = normalize(i.viewDir);

                float3 albedo = tex2D(_MainTex, uvMain).rgb * _Color.rgb;
                float metallic = tex2D(_MetallicMap, uvMetallic).r * _Metallic;
                float roughness = tex2D(_RoughnessMap, uvRoughness).r * _Roughness;
                float smoothness = 1.0 - roughness;
                float3 emission = tex2D(_EmissionMap, uvEmission).rgb * _Emission;

                float3 specColor;
                float oneMinusReflectivity;
                float3 diffColor = DiffuseAndSpecularFromMetallic(albedo, metallic, specColor, oneMinusReflectivity);

                UnityLight light;
                light.color = _LightColor0.rgb;
                light.dir = normalize(_WorldSpaceLightPos0.xyz);
                light.ndotl = saturate(dot(N, light.dir));

                UnityIndirect indirect;
                indirect.diffuse = UNITY_LIGHTMODEL_AMBIENT.rgb;
                indirect.specular = 0;

                float4 finalColor = UNITY_BRDF_PBS(diffColor, specColor, oneMinusReflectivity, smoothness, N, V, light, indirect);
                finalColor.rgb += emission;
                finalColor.a = _Color.a;
                return finalColor;
            }
            ENDCG
        }

        // パス2: ステンシル内かつ接触してるときだけ透明化
        Pass
        {
            Name "InsideStencil"
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            ZTest LEqual

            Stencil
            {
                Ref [_StencilID]
                ReadMask [_StencilID]
                Comp Equal
                Pass Keep
                WriteMask 0 
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityStandardUtils.cginc"
            #include "UnityPBSLighting.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MetallicMap;
            float4 _MetallicMap_ST;
            sampler2D _RoughnessMap;
            float4 _RoughnessMap_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            sampler2D _EmissionMap;
            float4 _EmissionMap_ST;

            float4 _Color;
            float _Metallic;
            float _Roughness;
            float _Emission;
            float4 _HitPosition;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w;
                o.worldPos = worldPos;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.uv = v.uv;
                o.tangentDir = worldTangent;
                o.bitangentDir = worldBitangent;
                o.normalDir = worldNormal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uvMain = TRANSFORM_TEX(i.uv, _MainTex);
                float2 uvMetallic = TRANSFORM_TEX(i.uv, _MetallicMap);
                float2 uvRoughness = TRANSFORM_TEX(i.uv, _RoughnessMap);
                float2 uvNormal = TRANSFORM_TEX(i.uv, _NormalMap);
                float2 uvEmission = TRANSFORM_TEX(i.uv, _EmissionMap);

                float3x3 TBN = float3x3(normalize(i.tangentDir), normalize(i.bitangentDir), normalize(i.normalDir));
                float3 normalMap = tex2D(_NormalMap, uvNormal).xyz * 2 - 1;
                float3 N = normalize(mul(normalMap, TBN));
                float3 V = normalize(i.viewDir);

                float3 albedo = tex2D(_MainTex, uvMain).rgb * _Color.rgb;
                float metallic = tex2D(_MetallicMap, uvMetallic).r * _Metallic;
                float roughness = tex2D(_RoughnessMap, uvRoughness).r * _Roughness;
                float smoothness = 1.0 - roughness;
                float3 emission = tex2D(_EmissionMap, uvEmission).rgb * _Emission;

                float3 specColor;
                float oneMinusReflectivity;
                float3 diffColor = DiffuseAndSpecularFromMetallic(albedo, metallic, specColor, oneMinusReflectivity);

                UnityLight light;
                light.color = _LightColor0.rgb;
                light.dir = normalize(_WorldSpaceLightPos0.xyz);
                light.ndotl = saturate(dot(N, light.dir));

                UnityIndirect indirect;
                indirect.diffuse = UNITY_LIGHTMODEL_AMBIENT.rgb;
                indirect.specular = 0;

                float4 finalColor = UNITY_BRDF_PBS(diffColor, specColor, oneMinusReflectivity, smoothness, N, V, light, indirect);
                finalColor.rgb += emission;

                finalColor.a = (_HitPosition.w > 0.5) ? 0 : _Color.a;
                return finalColor;
            }
            ENDCG
        }
    }

    FallBack "Standard"
}
