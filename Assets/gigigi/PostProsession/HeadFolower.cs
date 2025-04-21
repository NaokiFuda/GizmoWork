
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HeadFolower : UdonSharpBehaviour
{
    private VRCPlayerApi m_owner;

    void Start()
    {
        m_owner = Networking.GetOwner(gameObject);
    }

    private void Update()
    {
        transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + new Vector3(0, 0, 0);
        transform.rotation = m_owner.GetRotation() * Quaternion.AngleAxis(180, Vector3.up);
    }

    
}