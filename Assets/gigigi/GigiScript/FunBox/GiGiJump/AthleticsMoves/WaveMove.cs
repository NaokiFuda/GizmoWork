
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WaveMove : UdonSharpBehaviour
{
    [SerializeField] private Vector3 LeftWave = Vector3.left * 10.0f;
    [SerializeField] private Vector3 RightWave = Vector3.right * 10.0f;
    [SerializeField] private float ReturnTime = 3.0f;

    [SerializeField] private bool UpDown = false;
    [SerializeField] private Vector3 UpWave = Vector3.up * 10.0f;
    [SerializeField] private Vector3 DownWave = Vector3.down * 10.0f;

    private Vector3 XorY1;
    private Vector3 XorY2;
    private bool PLControll = false;
    private bool riftOn = false;


    private bool PlayerEnter = false;

    private float currentTime;

    void Start()
    {
        XorY1 = LeftWave;
        XorY2 = RightWave;
        if (UpDown)
        {
            XorY1 = DownWave;
            XorY2 = UpWave;
        }
    }
    void Update()
    {
        if (!riftOn) { return; }
        if (!Input.anyKey)
        {
            var player = Networking.LocalPlayer;
            if (player.IsPlayerGrounded()) { PLControll = false; }
                
        }
        if (PLControll)
        {
            Rigidbody rb = this.transform.GetComponent<Rigidbody>();
            var player = Networking.LocalPlayer;
            if (player.IsPlayerGrounded())
            {
                player.TeleportTo(player.GetPosition() + rb.velocity * Time.deltaTime, player.GetRotation());
                return;
            }
                player.SetVelocity(player.GetVelocity());
        }
    }
    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        Rigidbody rb = this.transform.GetComponent<Rigidbody>();
        if (currentTime > ReturnTime)
        {
            rb.velocity = XorY2;
            FlicktionStick();

            if (currentTime > ReturnTime * 2)
            {
                currentTime = 0.0f;
            }
        }
        else
        {
            rb.velocity = XorY1;
            FlicktionStick();
        }
    }
    public void FlicktionStick()
    {
        if (PlayerEnter)
        {
            Rigidbody rb = this.transform.GetComponent<Rigidbody>();
            var player = Networking.LocalPlayer;
            player.SetVelocity(rb.velocity);
        }
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
        player.SetVelocity(player.GetVelocity());
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!riftOn)
        {
            if (player != Networking.LocalPlayer) { return; }
            Rigidbody rb = this.transform.GetComponent<Rigidbody>();
            player.SetVelocity(player.GetVelocity() - rb.velocity);
        }
    }
    public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if (!riftOn) { return; }

        PlayerEnter = false;
        PLControll = true;
       
        Rigidbody rb = this.transform.GetComponent<Rigidbody>();
        var player = Networking.LocalPlayer;
        player.SetVelocity(player.GetVelocity() + rb.velocity);
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
