Shader "GiGiGi/AutoDeskInsideOnlyStencilReader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Roughness", Range(0.0, 1.0)) = 0.5
        _SpecGlossMap("Roughness Map", 2D) = "white" {}

        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        [Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0

         _StencilID("Stencil ID", Float) = 1
        _HitPosition("Hit Position", Vector) = (0,0,0,0)
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT RoughnessSetup
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry+5" }
        LOD 300

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma target 3.5

            // -------------------------------------
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
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
            #pragma target 3.5

            // -------------------------------------

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma target 3.5

            // -------------------------------------

            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Deferred pass
        Pass
        {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma target 3.0
            #pragma exclude_renderers nomrt


            // -------------------------------------
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature_local _PARALLAXMAP

            #pragma multi_compile_prepassfinal
            #pragma multi_compile_instancing

            #pragma vertex vertDeferred
            #pragma fragment fragDeferred

            #include "UnityStandardCore.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
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
                Unity_GlossyEnvironmentData glossIn;
                indirect.specular = 0;

                float4 finalColor = UNITY_BRDF_PBS(diffColor, specColor, oneMinusReflectivity, smoothness, N, V, light, indirect);
                finalColor.rgb += emission;

                finalColor.a = (_HitPosition.w > 0.5) ? 0 : _Color.a;
                return finalColor;
            }
            ENDCG
        }
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry+5" }
        LOD 300

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]


            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
            // SM2.0: NOT SUPPORTED shader_feature_local _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP

            #pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
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
            #pragma target 2.0
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            // SM2.0: NOT SUPPORTED #pragma shader_feature_local _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature_local _PARALLAXMAP
            #pragma skip_variants SHADOWS_SOFT

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma target 2.0
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma skip_variants SHADOWS_SOFT
            #pragma multi_compile_shadowcaster

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            Stencil
            {
                Ref [_StencilID]
                Comp NotEqual
                Pass Keep
                ReadMask [_StencilID]
                WriteMask 0 
            }

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature_local _METALLICGLOSSMAP
            #pragma shader_feature_local _SPECGLOSSMAP
            #pragma shader_feature_local _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
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
                Unity_GlossyEnvironmentData glossIn;
                indirect.specular = 0;

                float4 finalColor = UNITY_BRDF_PBS(diffColor, specColor, oneMinusReflectivity, smoothness, N, V, light, indirect);
                finalColor.rgb += emission;

                finalColor.a = (_HitPosition.w > 0.5) ? 0 : _Color.a;
                return finalColor;
            }
            ENDCG
        }
    }


    FallBack "VertexLit"
    CustomEditor "AutodeskInteractiveShaderGUI"
}
