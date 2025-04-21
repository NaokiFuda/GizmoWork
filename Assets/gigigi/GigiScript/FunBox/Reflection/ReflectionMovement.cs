
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections;

public class ReflectionMovement : UdonSharpBehaviour
{
    public float rayDistance;

    void Update()
    {
        transform.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        var HeadPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        RaycastHit hit;
        if (Physics.Raycast(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position, transform.forward, out hit, rayDistance)) 
        {
            if(Vector3.Angle(Vector3.right, hit.normal)== 0 || Vector3.Angle(Vector3.right, hit.normal) == 180)
            { transform.position = new Vector3(hit.point.x, HeadPos.y, HeadPos.z); }
            if (Vector3.Angle(Vector3.back, hit.normal) == 0 || Vector3.Angle(Vector3.back, hit.normal) == 180)
            { transform.position = new Vector3(HeadPos.x, HeadPos.y, hit.point.z);  }
        }
    }
}
