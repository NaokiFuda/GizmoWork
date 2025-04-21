
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class actSignal : UdonSharpBehaviour
{
	[SerializeField] GameObject[] Signal;
	private int i = 0;
	private int SignalNum = 0;

	[SerializeField] private bool isObj = false;
	[SerializeField] private bool isPlayer = true;
	[SerializeField] private bool isLocal = false;
	[SerializeField] private bool isExit = false;
	[SerializeField] private bool isEnter = true;
	[SerializeField] private bool noReciver = false;
	[SerializeField] private bool onlyOnce = false;
	[SerializeField] private bool isTrigger = false;
	private bool first = true;

	private bool act = false;
	[SerializeField] private bool isact01 = true;
	[SerializeField] private bool isact02 = false;
	[SerializeField] private bool isact03 = false;
	Animator animator;

	void Start()
	{
		SignalNum = Signal.Length;
	}

	public override void OnPlayerTriggerEnter(VRCPlayerApi player)
	{
		if (player != Networking.LocalPlayer)
		{
			return;
		}
		if (isObj == true)
		{ if (isPlayer == false) { return; } }
		if (isExit == true)
		{ if (isEnter == false) { return; } }
		if (noReciver == false)
		{
			for (i = 0; i < SignalNum; i++)
			{
				if (onlyOnce == true) { if (first == false) { return; } }

				if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
				{
					Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
				}
				if (isLocal == true)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act03");
					}
				}
				if (isLocal == false)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act03");
					}
				}
				if (i == SignalNum - 1)
				{
					first = false;
				}
			}
		}
		if (noReciver == true)
		{
			if (isLocal == true)
			{
				SendCustomEvent("Activate");
			}
			if (isLocal == false)
			{
				SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
			}
		}

	}
	public override void OnPlayerTriggerExit(VRCPlayerApi player)
	{
		if (player != Networking.LocalPlayer)
		{
			return;
		}
		if (isObj == true)
		{ if (isPlayer == false) { return; } }
		if (isEnter == true)
		{ if (isExit == false) { return; } }
		if (noReciver == false)
		{
			for (i = 0; i < SignalNum; i++)
			{
				if (onlyOnce == true) { if (first == false) { return; } }

				if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
				{
					Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
				}
				if (isLocal == true)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act03");
					}
				}
				if (isLocal == false)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act03");
					}
				}
				if (i == SignalNum - 1)
				{
					first = false;
				}
			}
		}
		if (noReciver == true)
		{
			if (isLocal == true)
			{
				SendCustomEvent("Activate");
			}
			if (isLocal == false)
			{
				SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
			}
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (isPlayer == true)
		{ if (isObj == false) { return; } }
		if (isEnter == true)
		{ if (isExit == false) { return; } }
		if (isExit == false)
		{ if (isEnter == false) { return; } }
		if (noReciver == false)
		{
			for (i = 0; i < SignalNum; i++)
			{
				if (onlyOnce == true) { if (first == false) { return; } }

				if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
				{
					Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
				}
				if (isLocal == true)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act03");
					}
				}
				if (isLocal == false)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act03");
					}
				}
				if (i == SignalNum - 1)
				{
					first = false;
				}
			}
		}
		if (noReciver == true)
		{
			if (isLocal == true)
			{
				SendCustomEvent("Activate");
			}
			if (isLocal == false)
			{
				SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (isPlayer == true)
		{ if (isObj == false) { return; } }
		if (isEnter == true)
		{ if (isExit == false) { return; } }
		if (noReciver == false)
		{
			for (i = 0; i < SignalNum; i++)
			{
				if (onlyOnce == true) { if (first == false) { return; } }

				if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
				{
					Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
				}
				if (isLocal == true)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomEvent("Act03");
					}
				}
				if (isLocal == false)
				{
					if (isact01 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act01");
					}
					if (isact02 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act02");
					}
					if (isact03 == true)
					{
						UdonBehaviour Send = (UdonBehaviour)Signal[i].GetComponent(typeof(UdonBehaviour));
						Send.SendCustomNetworkEvent(NetworkEventTarget.All, "Act03");
					}
				}
				if (i == SignalNum - 1)
				{
					first = false;
				}
			}
		}
		if (noReciver == true)
		{
			if (isLocal == true)
			{
				SendCustomEvent("Activate");
			}
			if (isLocal == false)
			{
				SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
			}
		}
	}
	public void Activate()
	{
		for (i = 0; i < SignalNum; i++)
		{
			if (onlyOnce == true) { if (first == false) { return; } }

			if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
			{
				Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
			}
			animator = Signal[i].GetComponent<Animator>();
			if (isact01 == true)
			{
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
			if (isact02 == true)
			{
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
			if (isact03 == true)
			{
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

			if (i == SignalNum - 1)
			{
				first = false;
			}
		}
	}
}
