
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiJump : UdonSharpBehaviour
{
    [SerializeField] private Vector3 JumpSpeed = Vector3.up * 5.0f;
    [SerializeField] private float FallSpeed = 1.3f;

    private bool IsJump = false;
    private bool SecoundJump = false;
    private bool First = false;

    private Vector3 LandPos;

    private float currentTime;
    private float fallenTime;
    public float startTime = 1.0f;
    public float startTime0 = 0.3f;



    public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if(First)
        {
            First = false;
            return;
        }
        if (!IsJump)
        {
            if (Networking.LocalPlayer.GetVelocity().y >= 0)
            {
                var player = Networking.LocalPlayer;
                player.TeleportTo(player.GetPosition() + new Vector3(0, 0.1f, 0), player.GetRotation());
                player.SetVelocity(JumpSpeed); 
                IsJump = true;
                First = true;
                return;
            }
            if (Networking.LocalPlayer.GetVelocity().y < 0)
            {
                if (fallenTime < startTime0)
                {
                    var player = Networking.LocalPlayer;
                    player.SetVelocity(JumpSpeed);
                    IsJump = true;
                    First = true;
                    return;
                }
            }
        }
        if (!SecoundJump)
        {
            var player = Networking.LocalPlayer;
            player.SetVelocity(JumpSpeed);
            currentTime = 0;
            SecoundJump = true;
        }
    }

    private void Update()
    {
        if (IsJump)
        {
            currentTime += Time.deltaTime;
            if (currentTime > startTime)
            {
                var player = Networking.LocalPlayer;
                player.SetGravityStrength(FallSpeed);
            }
            if (Networking.LocalPlayer.GetVelocity().y == 0)
            {
                var player = Networking.LocalPlayer;
                player.SetGravityStrength(1.0f);
                IsJump = false;
                SecoundJump = false;
                currentTime = 0;
                fallenTime = 0;
            }
        }
        if (Networking.LocalPlayer.GetVelocity().y < 0)
        {
            fallenTime += Time.deltaTime;
        }
    }
}
