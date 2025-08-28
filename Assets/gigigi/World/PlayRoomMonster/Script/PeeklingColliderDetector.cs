
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
            if(action.IsPeekling && other.gameObject.layer == 29)
                action.SetAction(other, transform.position);
        }
        public void OnTriggerExit(Collider other)
        {
            if (action.IsPeekling && other.gameObject.layer == 29)
                action.SetAction(null, transform.position);
        }
        public void OnTriggerStay(Collider other)
        {
            if (action.IsPeekling && other.gameObject.layer == 29)
                action.AddPower(transform.position);
        }
        private void Update()
        {
            
        }
    }
}

