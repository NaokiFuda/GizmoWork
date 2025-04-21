
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class JoninerSignal : UdonSharpBehaviour
{
    [SerializeField] GameObject[] Signals;
    [SerializeField] bool IsLocal = true;
    [SerializeField] bool DoActive = false;
    private int i = 0;
    private int SignalNum = 0;

    void Start()
    {
        SignalNum = Signals.Length;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer)
        {
            return;
        }
        i = Signals.Length;

        if (i == 0)
        {
            if (IsLocal)
            {
                UdonBehaviour Send = (UdonBehaviour)this.gameObject.GetComponent(typeof(UdonBehaviour));
                Send.SendCustomEvent("Activate");
            }
            else
            {
                UdonBehaviour Send = (UdonBehaviour)this.gameObject.GetComponent(typeof(UdonBehaviour));
                Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
            }
        }

        if (IsLocal)
        {
            for (i = 0; i < SignalNum; i++)
            {
                if (IsLocal)
                {
                    if (!Networking.IsOwner(Networking.LocalPlayer, Signals[i].gameObject))
                    {
                        Networking.SetOwner(Networking.LocalPlayer, Signals[i].gameObject);
                    }
                    UdonBehaviour Send = (UdonBehaviour)Signals[i].GetComponent(typeof(UdonBehaviour));
                    Send.SendCustomEvent("Activate");
                }
            }
        }

        else
        {
            for (i = 0; i < SignalNum; i++)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
            }
        }
    }

    public void Activate()
    {
        if (DoActive)
        {
            i = Signals.Length;
            if (i == 0)
            {
                if (this.gameObject.activeSelf)
                {
                    this.gameObject.SetActive(false);
                }

                else
                {
                    this.gameObject.SetActive(true);
                }
            }
            else
            {
                for (i = 0; i < SignalNum; i++)
                {
                    if (Signals[i].gameObject.activeSelf)
                    {
                        Signals[i].gameObject.SetActive(false);
                    }

                    else
                    {
                        Signals[i].gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            i = Signals.Length;
           
            if (i == 0)
            {
                return;
            }
            for (i = 0; i < SignalNum; i++)
            {
                if (!Networking.IsOwner(Networking.LocalPlayer, Signals[i].gameObject))
                {
                    Networking.SetOwner(Networking.LocalPlayer, Signals[i].gameObject);
                }
                UdonBehaviour Send = (UdonBehaviour)Signals[i].GetComponent(typeof(UdonBehaviour));
                Send.SendCustomEvent("Activate");
            }
        }
    }
}
