
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SetPlayerButton : UdonSharpBehaviour
{
    public ImmersiveCore immersiveCore;
    public override void Interact()
    {
       Debug.Log(immersiveCore.GetPlayersName[0]);
    }
}
