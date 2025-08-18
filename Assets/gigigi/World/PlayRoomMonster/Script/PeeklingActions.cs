
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PlayRoomMonster
{
    public class PeeklingActions : UdonSharpBehaviour
    {
        [HideInInspector]public bool isMe;
        bool _isHold;
        public void SetAction(Collider other)
        {

        }
    }
}

