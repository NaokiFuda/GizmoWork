
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ActivatePass : UdonSharpBehaviour
{
    [SerializeField] private GameObject Signal;
    public void Activate()
    {
        UdonBehaviour Send = (UdonBehaviour)Signal.GetComponent(typeof(UdonBehaviour));
        Send.SendCustomEvent("Activate");
    }
}
