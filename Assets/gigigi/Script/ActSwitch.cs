
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ActSwitch : UdonSharpBehaviour
{
	[SerializeField] private GameObject Signal;
	Animator animator;
	private bool act = false;
	private bool first = false;


	public override void Interact()
	{

		if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
		}

		// PlayerAnchorについてるUdonスクリプトのActivateを呼ぶ
		UdonBehaviour Me = (UdonBehaviour)this.GetComponent(typeof(UdonBehaviour));
		Me.SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
	}

	public void Activate()
	{
		animator = this.gameObject.GetComponent<Animator>();
		act = animator.GetBool("active");
		animator.SetBool("active", !act);
		if (first == true)
		{
			if (!Networking.IsOwner(Networking.LocalPlayer, Signal.gameObject))
			{
				Networking.SetOwner(Networking.LocalPlayer, Signal.gameObject);
			}
			UdonBehaviour Send = (UdonBehaviour)Signal.GetComponent(typeof(UdonBehaviour));
			Send.SendCustomNetworkEvent(NetworkEventTarget.All, "DeActivate");
			first = false;
			return;
		}
		if (first == false)
		{
			if (!Networking.IsOwner(Networking.LocalPlayer, Signal.gameObject))
			{
				Networking.SetOwner(Networking.LocalPlayer, Signal.gameObject);
			}
			UdonBehaviour Send = (UdonBehaviour)Signal.GetComponent(typeof(UdonBehaviour));
			Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
			first = true;
		}
	}
}
