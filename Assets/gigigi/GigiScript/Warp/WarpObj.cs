
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WarpObj : UdonSharpBehaviour
{
    [SerializeField] GameObject Target;
    [SerializeField] GameObject MovePoint;
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer){return; }
        Target.transform.position = MovePoint.transform.position;
    }
}
