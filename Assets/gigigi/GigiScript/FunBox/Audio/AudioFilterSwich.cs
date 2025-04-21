
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AudioFilterSwich : UdonSharpBehaviour
{
    [SerializeField] AudioLowPassFilter[] targetAudio;
    bool stayInTrigger;
    BoxCollider hitCollider;
    float colliderHeight;

    private void Start()
    {
        hitCollider = GetComponent<BoxCollider>();
        colliderHeight = transform.position.y + hitCollider.center.y + hitCollider.size.y / 2;
    }
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer) return;

        Vector3 cameraPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        if (stayInTrigger)
        {
            if (colliderHeight < cameraPosition.y)
            {
                stayInTrigger = false;
                SendSignals();
            }
        }
        else if (colliderHeight > cameraPosition.y)
        {
            stayInTrigger = true;
            SendSignals();
        }
    }

    void SendSignals()
    {
        foreach (var signal in targetAudio)
            signal.enabled = !signal.enabled;
    }

}

