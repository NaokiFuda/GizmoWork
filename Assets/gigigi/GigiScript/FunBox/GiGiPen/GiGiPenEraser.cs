
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GiGiPenEraser : UdonSharpBehaviour
{
	
	public GameObject Pen;
	private ParticleSystem Ink;

	void Start()
	{
		Ink = Pen.GetComponent<ParticleSystem>();
	}
	public override void OnPickupUseDown()
	{
		SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
	}
	public override void OnPickupUseUp()
	{
		SendCustomNetworkEvent(NetworkEventTarget.All, "DeActivate");
	}
	public void Activate()
	{
		Ink.Play(true);
		var main = Ink.main;
		main.simulationSpeed = 0.01f;
		var emission = Ink.emission;
		emission.rateOverTime = 0.0f;
	}
	public void DeActivate()
	{
		var emission = Ink.emission;
		emission.rateOverTime = 10.0f;
		Ink.Pause(true);
		
	}
}
