
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace PlayRoomMonster
{
    public class PeeklingActions : UdonSharpBehaviour
    {
         bool _isPeekling;
        [SerializeField] GameManager gameManager;
        public bool IsPeekling { get => _isPeekling; set => _isPeekling = value; }
        Vector3 _holdPoint;
        bool _isHold;
        public void SetAction(Collider other, Vector3 handPos)
        {
            if (_isPeekling) 
            {
                _holdPoint = other.ClosestPoint(handPos);
                _isHold = true;
            }
            if(other == null)
            {
                _isHold = false;
            }
        }
        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            if(_isPeekling && value && _isHold) Networking.LocalPlayer.SetGravityStrength(0);
            if (_isPeekling && !value) gameManager.SetPlayersGravity(Networking.LocalPlayer, !_isPeekling);
        }
        public void AddPower(Vector3 handPos)
        {
            if (_isPeekling)
            {
                Networking.LocalPlayer.SetVelocity(handPos - _holdPoint);
            }
        }
    }
}

