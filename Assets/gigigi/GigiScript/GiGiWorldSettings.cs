
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiWorldSettings : UdonSharpBehaviour
{
    public float walkSpeed = 2; // 歩く速度
    public float runSpeed = 5;  // 走る速度
    public float gravity = 1;   // 重力
    public float jump = 3;      // ジャンプの強さ

    void Start()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        // エディターで動かすとプレイヤーがいないためnullチェックしておく。
        if (localPlayer == null) return;

        localPlayer.SetJumpImpulse(jump);
        localPlayer.SetWalkSpeed(walkSpeed);
        localPlayer.SetRunSpeed(runSpeed);
        localPlayer.SetGravityStrength(gravity);
    }
}
