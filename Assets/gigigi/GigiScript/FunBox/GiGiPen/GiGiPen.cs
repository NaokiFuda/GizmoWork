
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GiGiPen : UdonSharpBehaviour
{
    private Vector3 CurrentPos;
    private Vector3 LatePos;
    private float distance = 0;
    private GameObject Pen;
    private ParticleSystem Ink;

    void Start()
    {
        Pen = transform.GetChild(0).gameObject;
        Ink = Pen.GetComponent<ParticleSystem>();
    }
    void Update()
    {
        CurrentPos = Pen.gameObject.transform.position;
        distance = (CurrentPos - LatePos).magnitude * 600;
        var main = Ink.main;
        main.simulationSpeed = distance;
        LatePos = Pen.gameObject.transform.position;
    }
    public override void OnPickupUseDown()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, Pen.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, Pen.gameObject);
        }
        if (!Networking.IsOwner(Networking.LocalPlayer, transform.GetChild(4).gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, transform.GetChild(4).gameObject);
        }
        SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
        
    }
    public override void OnPickupUseUp()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, Pen.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, Pen.gameObject);
        }
        if (!Networking.IsOwner(Networking.LocalPlayer, transform.GetChild(4).gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, transform.GetChild(4).gameObject);
        }
        SendCustomNetworkEvent(NetworkEventTarget.All, "DeActivate");
    }
    public void Activate()
    {
        Ink.Play(true);
    }
    public void DeActivate()
    {
        Ink.Pause(true);
    }
    
}
