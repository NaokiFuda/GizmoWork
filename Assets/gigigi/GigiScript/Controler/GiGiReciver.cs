
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiReciver : UdonSharpBehaviour
{
	Animator animator;
	private bool act = false;

	void Start()
	{
		animator = this.gameObject.GetComponent<Animator>();
	}

	public void Activate()
	{

		act = animator.GetBool("active");
		animator.SetBool("active", true);
	}

	public void DeActivate()
	{

		act = animator.GetBool("active");
		animator.SetBool("active", false);
	}
}
