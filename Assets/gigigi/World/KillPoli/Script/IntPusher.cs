
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IntPusher : UdonSharpBehaviour
{
    [SerializeField] int _int = 0;
    [SerializeField] private GameObject Signal;

    public override void Interact()
    {
        SendCustomNetworkEvent(NetworkEventTarget.Owner, "IntPush");
    }

    public void IntPush()
    {
        UdonBehaviour Send = (UdonBehaviour)Signal.GetComponent(typeof(UdonBehaviour));
        var PushedInt = (int)Send.GetProgramVariable("_PushInt");
        if (PushedInt == _int)
        {
            Send.SendCustomEvent("PushCounter");
            return;
        }
        Send.SetProgramVariable("_PushInt", _int);
        Send.SendCustomEvent("PushCounter");
    }
}
