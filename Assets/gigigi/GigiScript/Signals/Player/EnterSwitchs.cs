
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class EnterSwitchs : UdonSharpBehaviour
{
	[SerializeField] GameObject[] Signals;
	private int i = 0;
	private int SignalNum = 0;

	[SerializeField] bool OnlyEnter = false;
	[SerializeField] bool IsLocal = false;
	[SerializeField] bool IsReverse = false;
	[SerializeField] bool IsAutomatic = true;
	[SerializeField] bool OnlyActive = false;
	[SerializeField] bool OnlyDeActive = false;
	[SerializeField] bool KillOwn = false;
	[SerializeField] bool KillOwnWithExit = false;

	Animator animator;

	void Start()
	{
		SignalNum = Signals.Length;
	}

	public override void OnPlayerTriggerEnter(VRCPlayerApi player)
	{
		if (player != Networking.LocalPlayer)
		{
			return;
		}
		i = Signals.Length;

		if (IsLocal)
		{
			if (i == 0)
			{
				this.gameObject.SetActive(false);
			}
			for (i = 0; i < SignalNum; i++)
			{
                if (OnlyActive) { Signals[i].gameObject.SetActive(true); continue; }
				if (OnlyDeActive) { Signals[i].gameObject.SetActive(false); continue; }
				if (IsAutomatic)
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
				else
                {
					if (IsReverse) { Signals[i].gameObject.SetActive(false); }
					else { Signals[i].gameObject.SetActive(true); }
				}
			}
			if (KillOwn) { this.gameObject.SetActive(false); }
		}

		else
		{
			SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
		}
		
	}
	public override void OnPlayerTriggerExit(VRCPlayerApi player)
	{
		if (OnlyEnter)
		{ return; }
		if (player != Networking.LocalPlayer)
		{
			return;
		}
		i = Signals.Length;

		if (IsLocal)
		{
			if (i == 0)
			{
				this.gameObject.SetActive(false);
			}
			for (i = 0; i < SignalNum; i++)
			{
				if (OnlyActive) { Signals[i].gameObject.SetActive(true); continue; }
				if (OnlyDeActive) { Signals[i].gameObject.SetActive(false); continue; }
				if (IsAutomatic)
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
				else
				{
					if (IsReverse) { Signals[i].gameObject.SetActive(true); }
					else { Signals[i].gameObject.SetActive(false); }
				}
			}
			if (KillOwnWithExit) { this.gameObject.SetActive(false); }
		}

		else
		{
			SendCustomNetworkEvent(NetworkEventTarget.All, "DeActivate");
		}
	}
	public void Activate()
	{
		for (i = 0; i < SignalNum; i++)
		{
			if (OnlyActive) { Signals[i].gameObject.SetActive(true); continue; }
			if (OnlyDeActive) { Signals[i].gameObject.SetActive(false); continue; }
			if (IsAutomatic)
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
			else
			{
				if (IsReverse) { Signals[i].gameObject.SetActive(false); }
				else { Signals[i].gameObject.SetActive(true); }
			}
		}
		if (KillOwn) { this.gameObject.SetActive(false); }
	}
	public void DeActivate()
	{
		for (i = 0; i < SignalNum; i++)
		{
			if (OnlyActive) { Signals[i].gameObject.SetActive(true); continue; }
			if (OnlyDeActive) { Signals[i].gameObject.SetActive(false); continue; }
			if (IsAutomatic)
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
			else
			{
				if (IsReverse) { Signals[i].gameObject.SetActive(true); }
				else { Signals[i].gameObject.SetActive(false); }
			}
		}
		if (KillOwnWithExit) { this.gameObject.SetActive(false); }
	}
}