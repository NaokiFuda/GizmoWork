
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RayCastSignals : UdonSharpBehaviour
{
	[SerializeField] GameObject HeadAnchorObject;

	[SerializeField] GameObject[] Signals;
	[SerializeField] GameObject[] SwitchSignals;
	private int i = 0;
	private int j = 0;
	private int SignalNum = 0;
	private int SwitchSignalNum = 0;
	[SerializeField] float RayCastReach = 100f;
	[SerializeField] bool IsLocal = false;
	[SerializeField] bool KillOwn = false;
	[SerializeField] bool BoolSwitch = true;
	[SerializeField] bool ActivateSwitch = true;

	bool act = false;
	Animator animator;

	void Start()
	{
		SignalNum = Signals.Length;
		SwitchSignalNum = SwitchSignals.Length;
	}
	void Update() { 

		i = Signals.Length;
		
		Ray ray = new Ray(HeadAnchorObject.transform.position, HeadAnchorObject.transform.forward) ;
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, RayCastReach))
		{
			if (hit.collider != null && hit.collider.gameObject.layer == 27)
			{
				
				if (IsLocal)
				{
					if (BoolSwitch)
					{
						SendCustomEvent("Activate");
						Debug.Log("animate!");
					}
					else
					{
						if (i == 0)
						{
							this.gameObject.SetActive(false);
						}
						for (i = 0; i < SignalNum; i++)
						{
							if (Signals[i].gameObject.activeSelf)
							{
								Signals[i].gameObject.SetActive(false);
							}

							else
							{
								Signals[i].gameObject.SetActive(true);
							}
						}

						if (ActivateSwitch)
						{
							SendCustomEvent("Activate");
						}
					}
					if (KillOwn) { this.gameObject.SetActive(false); }
				}

				else
				{
					SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
				}
			}
		}
		
	}

	public void Activate()
	{
		i = Signals.Length;
		j = SwitchSignals.Length;
		if (BoolSwitch)
		{
			if (i == 0)
			{
				animator = this.gameObject.GetComponent<Animator>();
				act = animator.GetBool("active");
				animator.SetBool("active", !act);
			}
		}
		for (i = 0; i < SignalNum; i++)
		{
			if (BoolSwitch)
			{
				animator = Signals[i].gameObject.GetComponent<Animator>();
				act = animator.GetBool("active");
				animator.SetBool("active", !act);
			}
            else
            {
				if (Signals[i].gameObject.activeSelf)
				{
					Signals[i].gameObject.SetActive(false);
				}

				else
				{
					Signals[i].gameObject.SetActive(true);
				}
			}
		}
		if (ActivateSwitch)
		{
			for (j = 0; j < SignalNum; j++)
			{
				if (Signals[j].gameObject.activeSelf)
				{
					Signals[j].gameObject.SetActive(false);
				}

				else
				{
					Signals[j].gameObject.SetActive(true);
				}
			}
		}
		if (KillOwn) { this.gameObject.SetActive(false); }
	}
}
