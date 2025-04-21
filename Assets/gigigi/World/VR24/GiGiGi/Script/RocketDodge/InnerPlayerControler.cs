using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class InnerPlayerControler : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject respawn;
    private Vector3 respawnPoint;
    [Header("移動速度")]
    [SerializeField, Range(0.0f, 10.0f), Tooltip("足の速さ")] private float playerVelocity = 4.0f;
    [SerializeField, Tooltip("歩く速さ")] private float walkSpeedDegree = 0.3f;

    [Header("ジャンプ力")]
    [SerializeField, Range(0.5f, 10.0f)] private float jumpVelocity = 5.0f;
    [SerializeField, Tooltip("空中でもジャンプできるかどうか")] private int jumpLimit = 2;

    [SerializeField, Range(0.1f, 8.0f),Tooltip("ジャンプ力の最低保証")] private float jumpMinVelocity = 0.3f;
    [SerializeField, Tooltip("どれだけ早くジャンプ力が最大になるか")] private float jumpChargeTime = 0.15f;

    private bool bulletHit = false;

    public bool BulletHit
    {
        get => bulletHit;
        set => bulletHit=value;
    }

    private int jumpCounter = 0;

    private float plSpeed = 0.0f;
    private float jumpTimer = 0.0f;
    private float floatingTimer = 0.0f;
    private float attackTimer = 0.0f;
    private Vector3 jumpForce = new Vector3(0, 0, 0);
    private bool stack =false;

    private bool _isJumping = false;
    private bool _isGrounded = false;

    private bool _isWalking = false;
    private bool _isSprinting = false;
    private bool _isLeftTurning = false;
    private bool _isRightTurning = false;
    private bool _isStepingBack = false;
    private bool _isJumped = false;

    private bool _isCrashed = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        respawn.transform.position = respawnPoint;
    }

    void Update()
    {
        jumpTimer += Time.deltaTime;
        floatingTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        // 移動入力系
        plSpeed = playerVelocity * walkSpeedDegree;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            plSpeed = playerVelocity;
            _isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isSprinting = false;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (!_isSprinting)
            {
                animator.SetFloat("z_Axis", plSpeed / playerVelocity);
            }
            else
            {
                animator.SetFloat("z_Axis", plSpeed / plSpeed);
            }

            _isWalking = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            plSpeed *= -1.0f;
            if (!_isSprinting)
            {
                animator.SetFloat("z_Axis", plSpeed / playerVelocity);
            }
            else
            {
                animator.SetFloat("z_Axis", plSpeed / plSpeed * -1.0f);
            }
            _isStepingBack = true;
            
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            animator.SetFloat("z_Axis", 0.0f);
        }

        //↓左右移動　↑前後移動
        if (Input.GetKey(KeyCode.D))
        {
            if (!_isSprinting)
            {
                animator.SetFloat("x_Axis", plSpeed / playerVelocity);
            }
            else
            {
                animator.SetFloat("x_Axis", plSpeed / plSpeed);
            }
            _isRightTurning = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            plSpeed *= -1.0f;
            if (!_isSprinting)
            {
                animator.SetFloat("x_Axis", plSpeed / playerVelocity);
            }
            else
            {
                animator.SetFloat("x_Axis", plSpeed / plSpeed * -1.0f);
            }
            _isLeftTurning = true;
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            animator.SetFloat("x_Axis", 0.0f);
        }

        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded)
            {
                jumpTimer = 0.0f;
                //triggerでcrounchJump(ジャンプ前の屈伸)を発火させようと思ったらtriggerクソ挙動するのでboolにした
                animator.SetBool("crounchJump", true);
            }
            else if(jumpCounter > 0)
            {
                jumpTimer = 0.0f;
                animator.SetBool("crounchJump", true);
            }
            

        }

        if (Input.GetKey(KeyCode.Space) && jumpTimer <= jumpChargeTime)
        {
            //キー入力したときの最低保証のジャンプ力がjumpMinVelocity。最大ジャンプ力jumpVelocityになるにつれjumpMinVelocityが限りなく0になる
            jumpForce = new Vector3(0.0f, jumpVelocity * rb.mass * jumpTimer / jumpChargeTime + jumpMinVelocity * rb.mass * (1.0f - jumpTimer / jumpChargeTime), 0.0f);
            
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isJumped = true;
            _isJumping = true;
            animator.SetBool("isJumping", true);
            animator.SetBool("isGrounded", false);
            _isGrounded = false;
        }
        //ラケットを振る
        if (Input.GetKeyDown(KeyCode.R))
        {
            attackTimer = 0.0f;
            animator.SetBool("attack", true);
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (attackTimer> 0.01f) {
                animator.SetBool("attack", false);
            }
        }


        //落下判定
        if (floatingTimer > 0.5f)
        {
            animator.SetBool("isGrounded", false);
            _isGrounded = false;
        }
        if (floatingTimer > 10.0f)
        {
            transform.position = respawnPoint;
        }

    }

    void FixedUpdate() 
    {
        if (_isRightTurning)
        {
            if (_isStepingBack)
            {
                rb.AddRelativeForce(new Vector3(plSpeed*-1.0f, 0.0f, 0.0f) * rb.mass, ForceMode.Impulse);
            }
            else
            {
                rb.AddRelativeForce(new Vector3(plSpeed, 0.0f, 0.0f) * rb.mass, ForceMode.Impulse);
            } 
            _isRightTurning = false;
        }
        if (_isLeftTurning)
        {
            if (_isStepingBack)
            {
                rb.AddRelativeForce(new Vector3(plSpeed * -1.0f, 0.0f, 0.0f) * rb.mass, ForceMode.Impulse);
            }
            else
            {
                rb.AddRelativeForce(new Vector3(plSpeed, 0.0f, 0.0f) * rb.mass, ForceMode.Impulse);
            }

            if (_isWalking || _isStepingBack)
            {
                rb.AddRelativeForce(new Vector3(0.0f, 0.0f, plSpeed * -2.0f) * rb.mass, ForceMode.Impulse);
            }
            _isLeftTurning = false;
        }
        if (_isWalking)
        {
            if (!stack)
            {
                rb.AddRelativeForce(new Vector3(0.0f, 0.0f, plSpeed) * rb.mass, ForceMode.Impulse);
            }

            _isWalking = false;
        }

        if (_isStepingBack)
        {
            rb.AddRelativeForce(new Vector3(0.0f, 0.0f, plSpeed) * rb.mass, ForceMode.Impulse);
            _isStepingBack = false;
        }
        
        if (_isJumped)
        {
            animator.SetBool("crounchJump", false);
            if (jumpCounter > 0)
            {
                animator.SetFloat("jump", jumpTimer / jumpChargeTime);
                rb.AddRelativeForce(jumpForce, ForceMode.Impulse);
                jumpCounter--;
            }
            _isJumped = false;
        }
        if (_isCrashed)
        {
            if (floatingTimer < 0.1f)
            {
                rb.AddRelativeForce(Vector3.up  * rb.mass, ForceMode.Impulse);
                rb.AddTorque(Vector3.up * rb.mass, ForceMode.Impulse);
            }
            
        }
    }

    //接地判定
    private void OnCollisionEnter(Collision collision)
    {
        floatingTimer = 0.0f;
        if (collision.contacts[0].normal.y > 0.70)
        {
            if (!_isGrounded && _isJumping)
            {
                _isJumping = false;
                animator.SetFloat("jump", 0.0f);
                animator.SetBool("isJumping", false);
            }
            animator.SetBool("isGrounded", true);
            _isGrounded = true;
        }

        if (_isCrashed)
        {
            Debug.Log("oh!!");
            transform.localRotation = new Quaternion(0, -1, 0, 0);
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            _isCrashed = false;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        floatingTimer = 0.0f;
        if (collision.contacts[0].normal.y > 0.70)
        {
            if (!_isJumping && !_isGrounded)
            {
                animator.SetBool("isGrounded", true);
                _isGrounded = true;
            }
        }
        if (_isGrounded && jumpCounter != jumpLimit)
        {
            jumpCounter = jumpLimit;
        }
        if (collision.contacts[0].normal.y < 0.5)
        {
            stack = true;
            rb.AddRelativeForce((collision.contacts[0].point - transform.position), ForceMode.Impulse);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        stack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //BulletHit処理
        if (other.gameObject.tag == "Bullet")
        {

            rb.constraints = RigidbodyConstraints.None;

            _isCrashed = true;
        }
    }
}