
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GiGiPenMove : UdonSharpBehaviour
{
	public GameObject Pen;
	private ParticleSystem Ink;

	void Start()
	{
		Ink = Pen.GetComponent<ParticleSystem>();
	}
	public override void OnPickup()
	{
		if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
		}

		UdonBehaviour Send = (UdonBehaviour)this.GetComponent(typeof(UdonBehaviour));
		Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
	}

	public void Activate()
	{
		Ink.Play(true);
		var main = Ink.main;
		main.simulationSpeed = 0;
		var emission = Ink.emission;
		emission.rateOverTime = 0.0f;

	}
	public override void OnDrop() 
	{
		if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
		}

		UdonBehaviour Send = (UdonBehaviour)this.GetComponent(typeof(UdonBehaviour));
		Send.SendCustomNetworkEvent(NetworkEventTarget.All, "DeActivate");
	}
	public void DeActivate()
	{
		var emission = Ink.emission;
		emission.rateOverTime = 10.0f;
		Ink.Pause(true);
	}
}
