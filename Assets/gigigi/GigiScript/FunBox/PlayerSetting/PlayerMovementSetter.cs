
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
    [SerializeField] PlaySEsOneShot[] actions;
    [SerializeField] UdonBehaviour[] trigerActions;
    [SerializeField] String[] actionsName;
    [SerializeField] float waterDepth = 0.1f;
    [SerializeField] float fliction = 0.01f;

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

    public void Deactivate()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null) return;
        localPlayer.SetJumpImpulse(worldSettings.jump);
        localPlayer.SetWalkSpeed(worldSettings.walkSpeed);
        localPlayer.SetRunSpeed(worldSettings.runSpeed);
        localPlayer.SetGravityStrength(worldSettings.gravity);
    }
    public void OnPlayerStay(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            int localID =player.playerId - 1;
            var headPos = player.GetBonePosition(HumanBodyBones.Head);
            var hipPos = player.GetBonePosition(HumanBodyBones.Hips);
            var massHeight = borderHeight - ((headPos + hipPos)/2).y + Mathf.Abs(headPos.y - hipPos.y);
            var floatVel = Vector3.zero;
            if (_doSink)
            {
                if (massHeight > waterDepth){ _doSink = false; }
            }
            else
            {
                if (massHeight < 0) massHeight = 0;
                if(massHeight > waterDepth)
                floatVel = Vector3.up * massHeight;
                if (massHeight > 5) return;
            }
            var velocity = player.GetVelocity();
            velocity.y = Mathf.Min(0, velocity.y);
            velocity = Vector3.Lerp(velocity, Vector3.zero , fliction);
            if (velocity.sqrMagnitude <= 3) player.SetVelocity(velocity +  floatVel);

            if (trigerJump)
            {
                var headTransform = playerRayManager.GetPlayerHeadTransform();
                var junpForce = headTransform.forward * inputMovement.x +  Vector3.up * jump / 1.2f + headTransform.right * inputMovement.y;
                Networking.LocalPlayer.SetVelocity(junpForce);
                TrigerAction();

                actions[localID].PlayAction(actions[localID].GetHitSEIndex(),true);
                
            }
            if (inputMovement != Vector2.zero) 
            {
                TrigerAction();

                actions[localID].PlayAction(actions[localID].GetStaySEIndex(), inputMovement.magnitude,true);
                
            }
        }
    }
    bool _doSink;
    public void OnPlayerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            _doSink = true;
        }
    }
    public void OnPlayerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            _doSink = false;
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
