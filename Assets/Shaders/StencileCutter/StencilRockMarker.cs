
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StencilRockMarker : UdonSharpBehaviour
{
    [SerializeField] Renderer targetRenderer;
    [SerializeField] Renderer stencilRenderer;
    public string propertyName = "_HitPosition";
    private MaterialPropertyBlock _mpb;
    void Start()
    {
        _mpb = new MaterialPropertyBlock();
        if (targetRenderer == null) targetRenderer= GetComponent<Renderer>();
        if(stencilRenderer == null) stencilRenderer = transform.GetComponentInChildren<Renderer>();
        ApplyHitPosition(new Vector4(0, 0, 0, 1));

    }
    void ApplyHitPosition(Vector4 pos)
    {
        if (targetRenderer == null) return;
        targetRenderer.GetPropertyBlock(_mpb);
        _mpb.SetVector(propertyName, pos);
        targetRenderer.SetPropertyBlock(_mpb);
        stencilRenderer.SetPropertyBlock(_mpb);
    }
}
