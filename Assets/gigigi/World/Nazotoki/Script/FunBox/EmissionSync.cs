using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EmissionSync : UdonSharpBehaviour
{
    public Material emissionMaterial; // Emission 用マテリアル
    public Renderer screenRenderer;   // iwasync のメッシュレンダラー

    void Start()
    {
        // 動画の RenderTexture を取得（iwasync の mainTexture として）
        Texture videoTex = screenRenderer.material.mainTexture;

        // Emission マテリアルに設定
        emissionMaterial.SetTexture("_EmissionMap", videoTex);
        emissionMaterial.EnableKeyword("_EMISSION");
    }
}
