
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SendCustomEvent : UdonSharpBehaviour
{
    [SerializeField] string methodName = "Activate";
    [SerializeField] GameObject[] targets;
    [SerializeField] int targetUdonIndex = 0;
    [SerializeField] bool isLocal = true;

    private void Start()
    {
        if (targets == null) { Debug.Log(this + " gameObject has no target Objects"); return; }
    }
    public override void Interact()
    {
        foreach (GameObject target in targets)
        {
            UdonBehaviour[] udonBehaviours = target.GetComponents<UdonBehaviour>();

            if (isLocal) udonBehaviours[targetUdonIndex].SendCustomEvent(methodName);

            else udonBehaviours[targetUdonIndex].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, methodName);

        }
        
    }
}
