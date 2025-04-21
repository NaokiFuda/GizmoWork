
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GiGiPenEraseAll : UdonSharpBehaviour
{
	public GameObject Pen;
	private ParticleSystem Ink;

	void Start()
	{
		Ink = Pen.GetComponent<ParticleSystem>();
	}
	public override void Interact()
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
		Ink.Clear(true);
	}
}
