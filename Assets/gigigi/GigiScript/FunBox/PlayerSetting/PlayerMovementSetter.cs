
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using static VRC.SDKBase.VRCPlayerApi;

public class PlayerMovementSetter : UdonSharpBehaviour
{
    [SerializeField] float walkSpeed = 2; // 歩く速度
    [SerializeField] float runSpeed = 5;  // 走る速度
    [SerializeField] float gravity = 1;   // 重力
    [SerializeField] float jump = 3;      // ジャンプの強さ

    [SerializeField] GiGiWorldSettings worldSettings;
    [SerializeField] PlayerRayManager playerRayManager;
    [SerializeField] UdonBehaviour[] actions;
    [SerializeField] UdonBehaviour[] trigerActions;
    [SerializeField] String[] actionsName;
    public float waitmas;

    bool trigerJump = false;
    Vector2 inputMovement;
    public Transform senderTransform;
    public float borderHeight;

    public void Activate()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        // エディターで動かすとプレイヤーがいないためnullチェックしておく。
        if (localPlayer == null) return;

        if (localPlayer.GetJumpImpulse() == worldSettings.jump) localPlayer.SetJumpImpulse(jump);
        else localPlayer.SetJumpImpulse(worldSettings.jump);
        if (localPlayer.GetWalkSpeed() == worldSettings.walkSpeed) localPlayer.SetWalkSpeed(walkSpeed);
        else localPlayer.SetWalkSpeed(worldSettings.walkSpeed);
        if (localPlayer.GetRunSpeed() == worldSettings.runSpeed) localPlayer.SetRunSpeed(runSpeed);
        else localPlayer.SetRunSpeed(worldSettings.runSpeed);
        if (localPlayer.GetGravityStrength() == worldSettings.gravity) localPlayer.SetGravityStrength(gravity);
        else localPlayer.SetGravityStrength(worldSettings.gravity);
    }
    Vector3 lastPos;
    public void OnPlayerStay(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            if (_doSink)
            {
                if (borderHeight < player.GetAvatarEyeHeightAsMeters()/2) _doSink = false;
                Debug.Log(_doSink);
            }
            else
            {
                var Head = player.GetBonePosition(HumanBodyBones.Head);
                var hip = player.GetPosition();
                var pos = (Head + hip) / 2;
                if (waitmas == 0) waitmas = (Head - hip).y/3;
                var friction = lastPos - pos;
                friction.y = 0;
                var plVel = player.GetVelocity();
                var mass = borderHeight + waitmas;
                var floatVel = Vector3.up * gravity * mass * mass;
                if (mass < 0) floatVel = Vector3.zero;
                var waterVel = plVel + friction + floatVel;
                if (waterVel.y > 0)
                    if (mass > 0f && mass < 0.01f || waterVel.y > 0.5f) waterVel.y = 0;
                player.SetVelocity(waterVel);
                lastPos = (Head + hip) / 2;
            }

            if (trigerJump)
            {
                var head = playerRayManager.GetPlayerHeadTransform();
                var junpForce = head.forward * inputMovement.x +  Vector3.up * jump / 1.2f + head.right * inputMovement.y;
                Networking.LocalPlayer.SetVelocity(junpForce);
                TrigerAction();

                if (trigerActions.Length > 0)
                {
                    foreach (var triger in actions)
                    {
                        triger.SetProgramVariable<bool>("trigerJump", trigerJump);
                    }
                }
            }
            if (inputMovement != Vector2.zero) 
            {
                TrigerAction();

                if (trigerActions.Length > 0)
                {
                    foreach (var triger in actions)
                    {
                        triger.SetProgramVariable("inputMovement", inputMovement);
                    }
                }
            }
        }
    }
    bool _doSink;
    public void OnPlayerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            _doSink = true;
            Debug.Log("Enter!");
        }
    }
    public void OnPlayerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            _doSink = false;
            Debug.Log("Exit");
        }
    }

    public override void InputJump(bool value, UdonInputEventArgs args)
    {
        trigerJump = args.boolValue;
    }
    public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
    {
       inputMovement.y = value;
    }
    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        inputMovement.x = value;
    }

    public void TrigerAction()
    {
        for (int i=0; i< trigerActions.Length;i++)
        {
            var action = trigerActions[i];
            action.SendCustomEvent(actionsName[i]); 
            if(senderTransform !=null) action.SetProgramVariable<Transform>("senderTransform", senderTransform);
        }
    }
}
