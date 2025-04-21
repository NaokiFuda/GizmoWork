
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class GimmicSignal : UdonSharpBehaviour
{
    [SerializeField] Transform[] targets;
    [SerializeField] Transform[] telepotTo;
    [SerializeField] string methodName = "Activate";
    [SerializeField] string className = "";
    [SerializeField] int layerNum = 28;
    [SerializeField] bool sendSignal;
    [SerializeField] bool isLocal = true;
    [SerializeField] bool switchOnOff;
    [SerializeField] bool playerTeleport;
    [SerializeField] float timer;
    [SerializeField] float confirmTime = 3f;

    UdonBehaviour[][] sendsSignal;

    private void Start()
    {
        if (sendSignal)
        {
            sendsSignal = new UdonBehaviour[targets.Length][];
            for (int i = 0; i < targets.Length; i++)
            {
                sendsSignal[i] = targets[i].GetComponents<UdonBehaviour>();
            }
        }
    }


    public void SendSwitch()
    {
        foreach (Transform t in targets)
            t.gameObject.SetActive(!t.gameObject.activeSelf);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == layerNum)
        {
            timer += Time.deltaTime;
            if (timer > confirmTime)
            {
                if (sendSignal)
                {
                    foreach (UdonBehaviour[] sendSignal in sendsSignal)
                        foreach (UdonBehaviour send in sendSignal)
                                if (isLocal)
                                    send.SendCustomEvent(methodName);
                                else
                                    send.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, methodName);

                }

                if (switchOnOff)
                    if (isLocal)
                        SendSwitch();
                    else
                        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SendSwitch");

                if (playerTeleport)
                {
                    int i = 0;
                    if (targets.Length > 1)
                        i = Random.Range(0, targets.Length - 1);

                    Networking.LocalPlayer.TeleportTo(telepotTo[i].position, telepotTo[i].rotation);
                }
                timer = 0;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (!isLocal && other.gameObject.layer == layerNum)
            timer = 0;
    }
}
