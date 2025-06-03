using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(Renderer))]
public class StencilObjectPositionUpdater : UdonSharpBehaviour
{
    [SerializeField] Renderer targetRenderer; // 対象オブジェクトのRenderer
    [SerializeField] Renderer stencilRenderer;
    public string propertyName = "_HitPosition"; // シェーダー側のプロパティ名
    private MaterialPropertyBlock _mpb2;
    [SerializeField] int layerNumber;

    void Start()
    {
        _mpb2 = new MaterialPropertyBlock();
        if (stencilRenderer == null) GetComponent<Renderer>();
    }


    void ApplyHitPosition(Vector4 pos)
    {
        if (targetRenderer == null) return;
        targetRenderer.GetPropertyBlock(_mpb2);
        _mpb2.SetVector(propertyName, pos);
        targetRenderer.SetPropertyBlock(_mpb2);
        stencilRenderer.SetPropertyBlock(_mpb2);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerNumber)
        {
            targetRenderer = other.GetComponent<Renderer>();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == layerNumber)
        {
            ApplyHitPosition(new Vector4(0, 0, 0, 0)); // 無効化
            targetRenderer = null;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == layerNumber)
        {
            if (targetRenderer == null) targetRenderer = other.GetComponent<Renderer>();
            Vector4 hitPos = other.ClosestPoint(transform.position);
            hitPos.w = 1.0f; // ← 有効化スイッチ！
            ApplyHitPosition(hitPos);
        }
    }
}
