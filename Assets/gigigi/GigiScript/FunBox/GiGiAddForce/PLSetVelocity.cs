
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PLSetVelocity : UdonSharpBehaviour
{
    [SerializeField] private Vector3 vel = Vector3.up * 10.0f;
    public override void OnPlayerTriggerEnter(VRCPlayerApi player) {
        if (player != Networking.LocalPlayer) { return; }
        player.SetVelocity(vel);
    }
}
