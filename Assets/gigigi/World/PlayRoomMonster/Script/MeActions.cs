
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace PlayRoomMonster
{
    public class MeActions : UdonSharpBehaviour
    {
        [HideInInspector] public bool isMe;
        [SerializeField] GameManager gameManager;
        [SerializeField] MeColliderDetector[] enterPlayer;
        VRCPlayerApi _seizePlayer;
        bool _seizing;
        public bool seizing { get => _seizing; set => _seizing = value; }
        float _timer;
        [SerializeField] float timerLimit = 3.0f;


        private void FixedUpdate()
        {
            if (_seizing) 
            {
                SetPlayerHold();
            }
        }
        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            if (!args.boolValue) _seizing = false;

            if (_seizePlayer == null || _seizing) return;
            if (args.boolValue) { _seizing = true; _timer = 0; }
        }

        public void SetAction(VRCPlayerApi player)
        {
            _seizePlayer = player;
        }

        public void SetPlayerHold( float addtimer = 0 )
        {
            _timer += Time.deltaTime + addtimer;
            if(_timer >= timerLimit) _seizing = false;
            _seizePlayer.SetVelocity(Vector3.zero);
            _seizePlayer.TeleportTo(transform.position, transform.rotation);
        }
    }
}

