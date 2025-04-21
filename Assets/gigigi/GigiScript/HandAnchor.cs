
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using VRC.SDKBase;
using VRC.Udon;

public class HandAnchor : UdonSharpBehaviour
{
    [SerializeField] bool isLeftHand;
    [SerializeField] bool useFinger;
    [SerializeField] bool allowRotate;
    private bool m_isActive = false;
    private VRCPlayerApi m_owner;

    private CapsuleCollider[] fingers;

    private void FixedUpdate()
    {
        if (!m_isActive)
        {
            return;
        }
        if(isLeftHand) transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position + new Vector3(0, 0, 0);
        else transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position + new Vector3(0, 0, 0);
        if (allowRotate) transform.rotation = Quaternion.AngleAxis(180, Vector3.up) * m_owner.GetRotation();
        else transform.rotation = m_owner.GetRotation() * Quaternion.AngleAxis(180, Vector3.up);
        if (useFinger)
        {
            foreach (var finger in fingers)
            {
                if (isLeftHand)
                {
                    if (finger.name[0] == 'I')
                    {
                        finger.transform.position = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftIndexDistal);

                        finger.transform.rotation = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.LeftIndexDistal);
                    }
                    if (finger.name[0] == 'T') 
                    {
                        finger.transform.position = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftThumbDistal);

                        finger.transform.rotation = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.LeftThumbDistal);
                    }
                }
                else
                {
                    if (finger.name[0] == 'I')
                    {
                        finger.transform.position = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightIndexDistal);

                        finger.transform.rotation = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.RightIndexDistal);
                    }
                    if (finger.name[0] == 'T')
                    {
                        finger.transform.position = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightThumbDistal);

                        finger.transform.rotation = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.RightThumbDistal);
                    }
                }
            }
        }
    }

    public void Activate()
    {
        // その時点のオーナーに
        m_owner = Networking.GetOwner(gameObject);
        m_isActive = !m_isActive;
        if(useFinger)
        {
            fingers = transform.GetComponentsInChildren<CapsuleCollider>();
        }
    }
    public void DeActivate()
    { 
        m_isActive = false;
    }
}
