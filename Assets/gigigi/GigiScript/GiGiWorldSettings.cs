
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiWorldSettings : UdonSharpBehaviour
{
    public float walkSpeed = 2; // �������x
    public float runSpeed = 5;  // ���鑬�x
    public float gravity = 1;   // �d��
    public float jump = 3;      // �W�����v�̋���

    void Start()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;

        // �G�f�B�^�[�œ������ƃv���C���[�����Ȃ�����null�`�F�b�N���Ă����B
        if (localPlayer == null) return;

        localPlayer.SetJumpImpulse(jump);
        localPlayer.SetWalkSpeed(walkSpeed);
        localPlayer.SetRunSpeed(runSpeed);
        localPlayer.SetGravityStrength(gravity);
    }
}
