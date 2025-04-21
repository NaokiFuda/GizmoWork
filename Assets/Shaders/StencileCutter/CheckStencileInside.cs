using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CheckStencileInside : UdonSharpBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock block;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    private void OnTriggerStay(Collider other)
    {
        block.SetFloat("_IsInside", 1f);
        rend.SetPropertyBlock(block);
    }

    private void OnTriggerExit(Collider other)
    {
        block.SetFloat("_IsInside", 0f);
        rend.SetPropertyBlock(block);
    }
}
