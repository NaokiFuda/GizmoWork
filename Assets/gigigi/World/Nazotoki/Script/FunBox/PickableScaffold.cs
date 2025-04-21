
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PickableScaffold : UdonSharpBehaviour
{
    [SerializeField] Collider thisCol;
    public override void OnPickup()
    {
        thisCol.enabled = false;
    }
    public override void OnDrop()
    {
        thisCol.enabled = true;
    }
}
