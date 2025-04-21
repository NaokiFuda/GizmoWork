
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiSwitch : UdonSharpBehaviour
{
	Animator animator;
	private bool act = false;

	void Start()
	{
		animator = this.gameObject.GetComponent<Animator>();
	}

	public override void Interact()
	{

		act = animator.GetBool("active");
		animator.SetBool("active", !act);
	}
}
