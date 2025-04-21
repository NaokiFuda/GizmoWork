
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class FollowPlayer : UdonSharpBehaviour
{
    [SerializeField] GameObject[] followObjs;
    public GameObject[] GetFollowObjs { get { return followObjs; } }
    [SerializeField] bool isGroundAnchor;
    [SerializeField] bool isHandAnchor;
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (followObjs.Length > 0 && player == Networking.LocalPlayer)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, followObjs[player.playerId - 1].gameObject))
                Networking.SetOwner(Networking.LocalPlayer, followObjs[player.playerId - 1].gameObject);

            if (isGroundAnchor)
            {
                followObjs[player.playerId - 1].GetComponent<PlayerGroundAnchor>().SendCustomEvent("Activate");
                return;
            }

            else if (isHandAnchor)
            {
                followObjs[player.playerId - 1].GetComponent<HandAnchor>().SendCustomEvent("Activate");
                return;
            }
            else
            {
                followObjs[player.playerId - 1].GetComponent<GiGiPlayerAnchor>().SendCustomEvent("Activate");
                return;
            }
        }
    }
    
}