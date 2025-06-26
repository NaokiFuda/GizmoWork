
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class HeadDeny : UdonSharpBehaviour
{
    Collider myColloder;
    VRCPlayerApi playerData;
    VRCPlayerApi.TrackingData playerHead;
    private void Update()
    {
        if(myColloder == null)myColloder = GetComponent<Collider>();
        if (playerData == null)
        {
            playerData = Networking.LocalPlayer;
            playerHead = playerData.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        }
        if (myColloder.bounds.Contains(playerHead.position))
        {
            var aimPos = playerData.GetPosition();
            var startPos = playerHead.position;
            startPos.y = aimPos.y;
            playerData.TeleportTo(aimPos - startPos + aimPos, playerData.GetRotation());
        }
    }
}
