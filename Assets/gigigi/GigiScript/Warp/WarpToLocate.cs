
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WarpToLocate : UdonSharpBehaviour
{
    [SerializeField] Transform target;

    public void Activate()
    {
        Networking.LocalPlayer.TeleportTo(target.position, target.rotation);
    }
}
