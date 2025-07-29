
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class AvatarScalingResetSwitch : UdonSharpBehaviour
{
    [SerializeField] private float defaultAvatarScale = 1f;
    private GameObject vrcWorld = null;
    private float defaultJumpImpulse = 3f;
    private float defaultWalkSpeed = 2f;
    private float defaultRunSpeed = 4f;
    private float defaultStrafeSpeed = 2f;

    private void Start()
    {
        vrcWorld = GameObject.Find("VRCWorld");
        if (vrcWorld) {
            UdonBehaviour worldSettings = vrcWorld.GetComponent<UdonBehaviour>();
            if (worldSettings) {
                defaultJumpImpulse = (float)worldSettings.GetProgramVariable("jumpImpulse");
                defaultWalkSpeed = (float)worldSettings.GetProgramVariable("walkSpeed");
                defaultRunSpeed = (float)worldSettings.GetProgramVariable("runSpeed");
                defaultStrafeSpeed = (float)worldSettings.GetProgramVariable("strafeSpeed");
            }
        }
    }
    public override void Interact()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (!Utilities.IsValid(localPlayer)) return;
        localPlayer.SetAvatarEyeHeightByMultiplier(defaultAvatarScale);
        Physics.gravity = new Vector3(0, -9.81f * defaultAvatarScale, 0);
        localPlayer.SetJumpImpulse(defaultAvatarScale * defaultJumpImpulse);
        localPlayer.SetWalkSpeed(defaultAvatarScale * defaultWalkSpeed);
        localPlayer.SetRunSpeed(defaultAvatarScale * defaultRunSpeed);
        localPlayer.SetStrafeSpeed(defaultAvatarScale * defaultStrafeSpeed);
    }
}
