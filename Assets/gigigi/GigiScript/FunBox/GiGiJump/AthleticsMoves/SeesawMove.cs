
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SeesawMove : UdonSharpBehaviour
{
    private Vector3 LetePos ;
    private Vector3 AfterPos;
    [SerializeField] private Vector3 JumpSpeed = Vector3.up * 5.0f;

    private bool PLControll = false;
    private bool riftOn = false;


    private bool PlayerEnter = false;

    private float currentTime;

    void Start()
    {
       
    }
    void Update()
    {
        if (riftOn)
        {
            if (!Input.anyKey)
            {
                PLControll = false;
            }
        }
        if (!PlayerEnter) { return; }
        var player = Networking.LocalPlayer;
        Rigidbody rb = this.transform.GetComponent<Rigidbody>();
        AfterPos = player.GetPosition();
        if (AfterPos.y > LetePos.y)
        {
            player.SetVelocity(JumpSpeed );
        }
        LetePos = player.GetPosition();
    }
    void FixedUpdate()
    {
        
    }
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer) { return; }
        riftOn = true;
        if (!PLControll)
        {
            if (player != Networking.LocalPlayer) { return; }
            PlayerEnter = true;
        }
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player != Networking.LocalPlayer) { return; }
        riftOn = false;
        
        PlayerEnter = false;
        Rigidbody rb = this.transform.GetComponent<Rigidbody>();
        if (AfterPos.y > LetePos.y)
        {
            player.SetVelocity(player.GetVelocity() + rb.angularVelocity);
        }
    }
    public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if (!riftOn) { return; }
        PlayerEnter = false;
        PLControll = true;
        Rigidbody rb = this.transform.GetComponent<Rigidbody>();
        var player = Networking.LocalPlayer;
        player.TeleportTo(player.GetPosition() + new Vector3(0, 1f, 0), player.GetRotation());
        player.SetVelocity(player.GetVelocity() + rb.angularVelocity);
    }
    public override void InputMoveHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if (!riftOn) { return; }

        PlayerEnter = false;
        PLControll = true;

    }
    public override void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if (!riftOn) { return; }

        PlayerEnter = false;
        PLControll = true;

    }

}
