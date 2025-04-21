
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class actReciver : UdonSharpBehaviour
{
	[SerializeField] GameObject[] Signal;
	private int i = 0;
	private int SignalNum = 0;

	Animator animator;
	private bool act = false;
	[SerializeField] private bool isBypass = false;
	[SerializeField] private bool isTrigger = false;


	void Start()
	{
		animator = this.gameObject.GetComponent<Animator>();
		SignalNum = Signal.Length;
		if(SignalNum== 0)
        {
			SignalNum = 1;
		}
	}

	public void Act01()
	{
		for (i = 0; i < SignalNum; i++)
		{
			if (isBypass == true)
			{
				if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
				{
					Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
				}
				animator = Signal[i].GetComponent<Animator>();
			}
			if (isTrigger == false)
			{
				act = animator.GetBool("act01");
				animator.SetBool("act01", !act);
			}

			if (isTrigger == true)
			{
				animator.SetTrigger("act01");
			}
		}
	}
	public void Act02()
	{
		for (i = 0; i < SignalNum; i++)
		{
			if (isBypass == true)
			{
				if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
				{
					Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
				}
				animator = Signal[i].GetComponent<Animator>();
			}
			if (isTrigger == false)
			{
				act = animator.GetBool("act02");
				animator.SetBool("act02", !act);
			}
			if (isTrigger == true)
			{
				animator.SetTrigger("act02");
			}
		}
	}
	public void Act03()
	{
		for (i = 0; i < SignalNum; i++)
		{
			if (isBypass == true)
			{
				if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
				{
					Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
				}
				animator = Signal[i].GetComponent<Animator>();
			}
			if (isTrigger == false)
			{
				act = animator.GetBool("act03");
				animator.SetBool("act03", !act);
			}

			if (isTrigger == true)
			{
				animator.SetTrigger("act03");
			}
		}
	}

	
}
