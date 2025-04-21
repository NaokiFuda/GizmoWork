
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RespawnObj : UdonSharpBehaviour
{
	[SerializeField] GameObject[] Signal;
	private int i = 0;
	private int SignalNum = 0;

	private Vector3 resetPos = new Vector3(0 , 0 , 0);
	private Quaternion resetRot = new Quaternion(0, 0, 0, 0);
	[SerializeField] private bool IsLocal = true;

	void Start()
	{
		
		SignalNum = Signal.Length;

		for (i = 0; i < SignalNum; i++)
		{
			resetPos = Signal[i].gameObject.transform.position;
			resetRot = Signal[i].gameObject.transform.rotation;
		}
	}

	public override void Interact()
	{
		for (i = 0; i < SignalNum; i++)
		{
			if (!Networking.IsOwner(Networking.LocalPlayer, Signal[i].gameObject))
			{
				Networking.SetOwner(Networking.LocalPlayer, Signal[i].gameObject);
			}
			if (IsLocal == true)
			{
				Signal[i].gameObject.transform.position = resetPos;
				Signal[i].gameObject.transform.rotation = resetRot;
			}
			if (IsLocal == false)
			{
				SendCustomNetworkEvent(NetworkEventTarget.All, "Reset");
			}
		}
	}
	public void Reset()
	{
		Signal[i].gameObject.transform.position = resetPos;
		Signal[i].gameObject.transform.rotation = resetRot;
	}
}
