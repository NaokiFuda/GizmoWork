
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]

public class DeskTopPickUpFront : UdonSharpBehaviour
{
    bool _isVR;
    private void OnEnable()
    {
        if (Networking.LocalPlayer.IsUserInVR()) _isVR = true;
        else _isVR = false;
    }

    public override void Interact()
    {
        if (_isVR) { return; }
        else if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))  Networking.SetOwner(Networking.LocalPlayer, gameObject);
        else transform.transform.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
    }
}
