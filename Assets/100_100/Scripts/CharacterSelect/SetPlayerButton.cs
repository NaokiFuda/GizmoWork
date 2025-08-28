
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SetPlayerButton : UdonSharpBehaviour
{
    public ImmersiveCore immersiveCore;
    [HideInInspector] public int playerID = -1;
    [HideInInspector] public Transform syncObj;
    [HideInInspector] public Transform headFollower;
    public override void Interact()
    {
        var player = Networking.LocalPlayer;
        immersiveCore.AssignPlayer(player, playerID);
        if (headFollower.childCount != 0) headFollower.GetChild(0).parent = null;
        syncObj.parent = headFollower;
        syncObj.localPosition = Vector3.up * 0.3f;
        if (playerID > 0) syncObj.GetChild(0).localPosition = Vector3.zero;
        else syncObj.localPosition = Vector3.zero;
    }
}
