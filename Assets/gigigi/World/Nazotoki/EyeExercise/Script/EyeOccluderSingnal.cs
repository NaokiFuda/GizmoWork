
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class EyeOccluderSingnal : UdonSharpBehaviour
{
    [SerializeField] Transform eyeLevel;
    [SerializeField] VRC_Pickup pickup;
    [SerializeField] PlayerRayManager playerRayManager;
    [SerializeField] string methodName = "Active";
    [SerializeField] UdonBehaviour target;
    void Update()
    {
        if (pickup != null && pickup.IsHeld && pickup.currentPlayer == Networking.LocalPlayer)
        {
            float eyeHeight = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position.y;
            if(Mathf.Abs(eyeHeight - eyeLevel.position.y) < 0.1f)
            {
                Ray ray = playerRayManager.GetPlayerRay();
                if(Physics.Raycast(ray, out RaycastHit hit, 10f, 1 << 27) && hit.collider.gameObject.layer == 27)
                {
                    target.SendCustomEvent(methodName);
                }
            }
        }
    }
}
