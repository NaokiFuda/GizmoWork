
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class TackleSwitch : UdonSharpBehaviour
{
    [SerializeField] Transform[] targets;
    [SerializeField] Transform[] teleportList;
    [SerializeField] string methodName ="Activate";
    [SerializeField] bool sendSignal;
    [SerializeField] bool isLocal = true;
    [SerializeField] bool switchOnOff;
    [SerializeField] bool playerTeleport;
    [SerializeField] float timer;
    [SerializeField] float confirmTime= 3f;

    bool _triggerStay;
    Vector2 _movingSpeed;
    UdonBehaviour[][] sendsSignal;

    private void Start()
    {
        if (sendSignal && targets.Length >0)
        {
            sendsSignal = new UdonBehaviour[targets.Length][];
            for (int i = 0; i < targets.Length; i++)
            {
                sendsSignal[i] = targets[i].GetComponents<UdonBehaviour>();
            }
        }
    }

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if(player.isLocal)
        {
            _triggerStay = true;
            if (_movingSpeed != Vector2.zero)
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

                            Networking.LocalPlayer.TeleportTo(teleportList[i].position, teleportList[i].rotation);
                        }
                    timer = 0;
                }
            }
            else timer = 0;
        }
    }

    public void SendSwitch()
    {
        foreach (Transform t in targets)
            t.gameObject.SetActive(!t.gameObject.activeSelf);
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            _triggerStay = false;
            _movingSpeed = Vector2.zero;
        }
    }
    public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
    {
        if(_triggerStay) _movingSpeed.x = value;
    }
    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        if (_triggerStay)_movingSpeed.y = value;
    }
}
