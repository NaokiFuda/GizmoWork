
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GiGiActivator : UdonSharpBehaviour
{
	[SerializeField] private bool IsLocal = true;
	Animator animator;
	private bool act = false;

	void Start()
    {
		animator = this.gameObject.GetComponent<Animator>();
	}

	public void Activate()
	{
		if(IsLocal)
		{
			if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
			{
				Networking.SetOwner(Networking.LocalPlayer, gameObject);
			}
			SendCustomEvent( "Set");
			return;
		}
		if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, gameObject);
		}
		SendCustomNetworkEvent(NetworkEventTarget.All, "Set");
	}

	public void DeActivate()
	{
		if (IsLocal)
		{
			if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
			{
				Networking.SetOwner(Networking.LocalPlayer, gameObject);
			}
			SendCustomEvent("UnSet");
			return;
		}
		if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, gameObject);
		}
		SendCustomNetworkEvent(NetworkEventTarget.All, "UnSet");
	}

    public void Set()
    {

        act = animator.GetBool("active");
        animator.SetBool("active", !act);
    }

	public void UnSet()
	{
		act = animator.GetBool("deactive");
		animator.SetBool("deactive", !act);
		act = animator.GetBool("active");
		animator.SetBool("active", false);
	}
}
