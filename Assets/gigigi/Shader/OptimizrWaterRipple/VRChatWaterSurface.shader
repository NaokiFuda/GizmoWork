Shader "Custom/VRChatWaterSurface"
{
    Properties
    {
        _Color ("Water Color", Color) = (0.2, 0.5, 0.8, 0.6)
        _DepthColor ("Depth Color", Color) = (0.0, 0.3, 0.5, 0.8)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1.0
        _Glossiness ("Smoothness", Range(0, 1)) = 0.9
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1.0
        _WaveScale ("Wave Scale", Range(0, 5)) = 1.0
        _RefractionStrength ("Refraction Strength", Range(0, 1)) = 0.1
        _DepthDistance ("Depth Distance", Range(0, 10)) = 2.0
        _RippleScale ("Ripple Scale", Range(0, 10)) = 1.0
        _RippleSpeed ("Ripple Speed", Range(0, 10)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200
        
        GrabPass { "_GrabTexture" }
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:blend
        #pragma target 3.0
        
        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _GrabTexture;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float4 screenPos;
            float3 worldPos;
            float3 viewDir;
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _DepthColor;
        float _NormalStrength;
        float _WaveSpeed;
        float _WaveScale;
        float _RefractionStrength;
        float _DepthDistance;
        float _RippleScale;
        float _RippleSpeed;
        
        // Function to create ripple effect
        float2 createRipple(float2 uv, float time, float scale)
        {
            float2 center = float2(0.5, 0.5);
            float dist = distance(uv, center);
            float ripple = sin((dist * scale - time) * 6.28) * 0.05;
            return normalize(uv - center) * ripple;
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Time variables for animation
            float time = _Time.y * _WaveSpeed;
            
            // Sample normal map with animated UVs for flowing water effect
            float2 flowUV1 = IN.uv_NormalMap + float2(time * 0.05, time * 0.03) * _WaveScale;
            float2 flowUV2 = IN.uv_NormalMap + float2(-time * 0.06, -time * 0.04) * _WaveScale;
            
            float3 normal1 = UnpackNormal(tex2D(_NormalMap, flowUV1));
            float3 normal2 = UnpackNormal(tex2D(_NormalMap, flowUV2));
            float3 finalNormal = normalize(normal1 + normal2);
            finalNormal = float3(finalNormal.xy * _NormalStrength, finalNormal.z);
            
            // Create ripple effect
            float rippleTime = _Time.y * _RippleSpeed;
            float2 ripple1 = createRipple(IN.uv_MainTex, rippleTime, _RippleScale);
            float2 ripple2 = createRipple(IN.uv_MainTex + float2(0.25, 0.25), rippleTime * 0.8, _RippleScale * 1.2);
            float2 rippleOffset = ripple1 + ripple2;
            
            // Apply ripple to normal
            finalNormal.xy += rippleOffset;
            finalNormal = normalize(finalNormal);
            
            // Refraction effect using grab pass
            float4 screenPos = IN.screenPos;
            float2 refractionOffset = finalNormal.xy * _RefractionStrength;
            screenPos.xy = screenPos.xy / screenPos.w + refractionOffset;
            fixed4 refractionColor = tex2D(_GrabTexture, screenPos.xy);
            
            // Main texture with animated flow
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex + finalNormal.xy * 0.05);
            
            // Combine effects
            fixed4 waterColor = lerp(_DepthColor, _Color, saturate(dot(finalNormal, normalize(IN.viewDir)) * _DepthDistance));
            o.Albedo = c.rgb * waterColor.rgb;
            o.Emission = refractionColor.rgb * (1.0 - waterColor.a);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Normal = finalNormal;
            o.Alpha = waterColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}