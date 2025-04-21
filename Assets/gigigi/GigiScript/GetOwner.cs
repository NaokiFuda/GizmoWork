
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GetOwner : UdonSharpBehaviour
{
    [SerializeField] GameObject[] Signals;
    private int i = 0;
    private int SignalNum = 0;
    void Start()
    {
        SignalNum = Signals.Length;
    }
	public override void Interact()
	{
        i = Signals.Length;

        if (i == 0)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
        }

        for (i = 0; i < SignalNum; i++)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, Signals[i].gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, Signals[i].gameObject);
            }
        }
    }
}
