
using System.Runtime.InteropServices;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GizmoSwitch : UdonSharpBehaviour
{
    public UdonBehaviour[] actions;
    public string[] actionName;
    void Start()
    {
        if (Networking.LocalPlayer.displayName == "Gizmo-pants")
        {
            GetComponent<Collider>().enabled = true;
            GetComponent<Renderer>().enabled = true;
        }

    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Activator");
    }
    public void Activator()
    {

        for (int i = 0; i < actions.Length; i++)
        {
            var action = actions[i];
            action.SendCustomEvent( actionName[i]);
        }
    }
}
