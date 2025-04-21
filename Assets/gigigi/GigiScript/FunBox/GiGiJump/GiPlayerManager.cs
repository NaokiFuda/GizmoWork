
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiPlayerManager : UdonSharpBehaviour
{
    [Header("ジャンプ力")]
    [SerializeField, Range(0.5f, 10.0f)] private float jumpVelocity = 5.0f;
    //[SerializeField, Tooltip("空中でもジャンプできるかどうか")] private int jumpLimit = 2;

    [SerializeField, Range(0.1f, 8.0f), Tooltip("ジャンプ力の最低保証")] private float jumpMinVelocity = 0.3f;
    [SerializeField, Tooltip("どれだけ早くジャンプ力が最大になるか")] private float jumpChargeTime = 0.15f;

    private float jumpTimer = 0.0f;
    private float jumpForce = 0.0f;
    private bool _isJumped = false;
    private int jumpCounter = 0;

    Rigidbody rb;

    void Start()
    {
        
    }
    public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args) 
    {
        
    }
    void Update()
    {
        jumpTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Networking.LocalPlayer.IsPlayerGrounded())
            {
                jumpTimer = 0.0f;
            }
            else if (jumpCounter > 0)
            {
                jumpTimer = 0.0f;
            }
        }
        if (Input.GetKey(KeyCode.Space) && jumpTimer <= jumpChargeTime)
        {
            jumpForce = jumpVelocity *  jumpTimer / jumpChargeTime + jumpMinVelocity *  (1.0f - jumpTimer / jumpChargeTime);
            Networking.LocalPlayer.SetJumpImpulse(jumpForce);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isJumped = true;
        }
    }

    void FixedUpdate()
    {
        if (_isJumped)
        {
            if (jumpCounter > 0 && !Networking.LocalPlayer.IsPlayerGrounded())
            {
                Networking.LocalPlayer.SetVelocity(new Vector3(0, jumpForce, 0));
                //rb.AddRelativeForce(jumpForce, ForceMode.Impulse);

                jumpCounter--;
            }
            _isJumped = false;
        }
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player) 
    {
        if (player != Networking.LocalPlayer) { return; }

    }
}
