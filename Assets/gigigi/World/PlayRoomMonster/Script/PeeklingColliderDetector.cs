
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PlayRoomMonster
{
    public class PeeklingColliderDetector : UdonSharpBehaviour
    {
        [SerializeField] PeeklingActions action;
        public void OrTriggerEnter(Collider other)
        {
            if(action.isMe|| other.gameObject.layer == 29)
                action.SetAction(other);
        }
        public void OnTriggerExit(Collider other)
        {
            if (action.isMe || other.gameObject.layer == 29)
                action.SetAction(null);
        }
    }
}

