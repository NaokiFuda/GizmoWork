
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class HeadHitSignal : UdonSharpBehaviour
{
    [SerializeField] GameObject[] signals;
    [SerializeField] bool onlySendSignalFirstOne;
    [SerializeField] bool killSelf;
    [SerializeField] bool sendBorderHeight;
    bool stayInTrigger;
    BoxCollider hitCollider;
    float colliderHeight;
    float borderHeight;
    private void Start()
    {
        hitCollider = GetComponent<BoxCollider>();
        colliderHeight = transform.position.y + hitCollider.center.y + hitCollider.size.y/2;
    }
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer) return;

        Vector3 cameraPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        borderHeight = colliderHeight - cameraPosition.y;
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
        foreach (var signal in signals)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, signal.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, signal.gameObject);
            }
            var senders = signal.GetComponents<UdonBehaviour>();
            if (senders.Length > 1 && onlySendSignalFirstOne)
            {
                senders[0].SendCustomEvent("Activate");
                if (sendBorderHeight) senders[0].SetProgramVariable<float>("borderHeight", borderHeight);
                break;
            }
            foreach (var sender in senders)
            {
                sender.SendCustomEvent("Activate");
                if (sendBorderHeight) sender.SetProgramVariable<float>("borderHeight", borderHeight);
            }
        }
        if (killSelf)
        {
            this.gameObject.SetActive(false);
        }
    }
}
