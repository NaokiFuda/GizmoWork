
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiKey : UdonSharpBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 27)
        {
            UdonBehaviour Send = (UdonBehaviour)other.gameObject.GetComponent(typeof(UdonBehaviour));
            Send.SendCustomEvent("Activate");
        }
    }
}
