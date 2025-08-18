
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PlayRoomMonster
{
    public class TrapSystem : UdonSharpBehaviour
    {
        [SerializeField]GameManager gameManager;
        [SerializeField] MeActions meActions;
        bool _isTraped;
        Collider _thisCol;
        Vector3 _exitPos;
        private void Start()
        {
            _thisCol = GetComponent<Collider>();
        }
        public override void OnPlayerCollisionEnter(VRCPlayerApi player)
        {
            if (gameManager.GetMePlayer == player || !meActions.seizing) return;
            _isTraped = true;
        }

        public override void OnPlayerCollisionExit(VRCPlayerApi player)
        {
            if (gameManager.GetMePlayer == player || !_isTraped) return;
            _exitPos = player.GetPosition();
            SetPlayerInCage(player);
        }

        public void SetPlayerInCage(VRCPlayerApi player)
        {
            player.SetVelocity(Vector3.zero);
            var surface = _thisCol.ClosestPoint(player.GetPosition());
            var dir = (surface - _exitPos).normalized;
            player.TeleportTo(surface+ dir * 0.01f, player.GetRotation());
        }

    }
}