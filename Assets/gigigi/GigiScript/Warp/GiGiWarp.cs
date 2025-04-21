
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiWarp : UdonSharpBehaviour
{
	[SerializeField] private Transform target;


	public override void Interact()
	{
		var player = Networking.LocalPlayer;
		player.TeleportTo(target.position, target.rotation);


	}
}
