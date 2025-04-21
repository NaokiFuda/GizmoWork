
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiHitWarp : UdonSharpBehaviour
{
    [SerializeField] private Transform target;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {

        if (player != Networking.LocalPlayer)
        {
            return;
        }
        player.TeleportTo(target.position, target.rotation);
    }
}
