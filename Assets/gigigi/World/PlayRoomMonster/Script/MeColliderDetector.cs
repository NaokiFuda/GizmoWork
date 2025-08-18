
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PlayRoomMonster
{
    public class MeColliderDetector : UdonSharpBehaviour
    {
        [SerializeField] MeActions action;
        public override void OnPlayerCollisionEnter(VRCPlayerApi player)
        {
            if (!action.isMe || player == Networking.LocalPlayer  ) return;
            action.SetAction(player);
        }
        public override void OnPlayerCollisionExit(VRCPlayerApi player)
        {
            if (!action.isMe || player == Networking.LocalPlayer ) return;
            action.SetAction(null);
        }
    }
}

