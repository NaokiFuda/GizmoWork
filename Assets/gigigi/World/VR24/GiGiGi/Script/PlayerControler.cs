using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using System.Reflection;

public class PlayerControler : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    [Header("各種必須設定")]
    [SerializeField] GameObject fieldManager;
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

    [Header("camera操作")]
    [SerializeField, Range(0.01f, 0.06f)] private float mouseSenciblity = 0.03f;
    [SerializeField] private float playerHeight = 0.5f;
    [SerializeField, Tooltip("カメラを上下に向けた時の移動量")] private float cameraHeightVelocity = 0.1f;
    private float _cameraHeight = 0;
    private Vector3 _cameraPos= new Vector3(0,0,0);
    [SerializeField, Tooltip("カメラの初期位置")] private Vector3 _cameraDefPos = new Vector3(0.0f, 0.5f, -1.3f);
    private Vector3 _cameraRotateAround;
    private float _mouseX = 0;
    private float _mouseY = 0;
    private float _mouseRotateRate = 0;
    private float _sinX;
    private float _cosX;
    private bool first=true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        _cameraPos = transform.position + _cameraDefPos;
        mainCamera.transform.position = _cameraPos;
        respawnPoint = respawn.transform.position;
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
        else if (Input.GetKey(KeyCode.S))
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
        else if (Input.GetKey(KeyCode.A))
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
            if (_isJumping)
            {
                animator.SetBool("isJumping", false);
                _isJumping = false;
            }
            if (_isGrounded)
            {
                jumpTimer = 0.0f;
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
            animator.SetBool("crounchJump", false);
            animator.SetFloat("jump", jumpTimer / jumpChargeTime);
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
        if (floatingTimer > 5.0f)
        {
            transform.position = respawnPoint;
        }

        //視点操作
        if (first&& floatingTimer > 0.1f|| Input.anyKey)
        {
            first = false;
        }
        if (!first)
        {
            if (Input.GetAxis("Mouse X") != 0)
            {
                _mouseX = Input.GetAxis("Mouse X") * mouseSenciblity;
                _mouseRotateRate += _mouseX;
                _cosX = Mathf.Cos(_mouseRotateRate * -1.0f);
                _sinX = Mathf.Sin(_mouseRotateRate * -1.0f);
                _cameraRotateAround = new Vector3(_cameraDefPos.x * _cosX - _cameraDefPos.z * _sinX, 0.0f, _cameraDefPos.x * _sinX + _cameraDefPos.z * _cosX) - new Vector3(0, 0, _cameraDefPos.z);
                mainCamera.transform.Rotate(0, _mouseX * 57.2958f, 0, Space.World);
            }
            if (Input.GetAxis("Mouse Y") != 0)
            {
                _mouseY = Input.GetAxis("Mouse Y") * mouseSenciblity * -1.0f;
                mainCamera.transform.rotation = mainCamera.transform.rotation * new Quaternion(_mouseY, 0, 0, 1.0f - _mouseY);

                if (mainCamera.transform.localEulerAngles.x > 30 && mainCamera.transform.localEulerAngles.x < 180)
                {
                    mainCamera.transform.rotation = mainCamera.transform.rotation * new Quaternion(_mouseY * -1.0f, 0, 0, 1.0f - _mouseY);
                }
                else if (mainCamera.transform.localEulerAngles.x < 330 && mainCamera.transform.localEulerAngles.x >= 180)
                {
                    mainCamera.transform.rotation = mainCamera.transform.rotation * new Quaternion(_mouseY * -1.0f, 0, 0, 1.0f - _mouseY);
                }
            }

            //カメラのローカル回転角が0度から30度上下を向く時の補正
            if (mainCamera.transform.localEulerAngles.x <= 30 || mainCamera.transform.localEulerAngles.x <= 340)
            {
                if (mainCamera.transform.localEulerAngles.x <= 30 && mainCamera.transform.localEulerAngles.x > 20)
                {
                    if (_cameraHeight < playerHeight)
                        _cameraHeight += cameraHeightVelocity;
                }
                else if (mainCamera.transform.localEulerAngles.x >= 330 && mainCamera.transform.localEulerAngles.x < 340)
                {
                    if (_cameraHeight > -playerHeight)
                        _cameraHeight -= cameraHeightVelocity;
                }
                else
                {
                    if (_cameraHeight != 0)
                    {
                        if (_cameraHeight > 0)
                        {
                            _cameraHeight = _cameraHeight - cameraHeightVelocity;
                        }
                        if (_cameraHeight < 0)
                        {
                            _cameraHeight = _cameraHeight + cameraHeightVelocity;
                        }
                        else { }
                    }
                }
            }

            //カメラ位置=プレイヤーの現在位置 + カメラのデフォルト位置　+　カメラの高さ調節　+　カメラの回転　+　
            _cameraPos = transform.position + _cameraDefPos + new Vector3(0, _cameraHeight, 0) + _cameraRotateAround;
            mainCamera.transform.position = _cameraPos;
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
            transform.rotation = new Quaternion(0, mainCamera.transform.rotation.y, 0, mainCamera.transform.rotation.w);
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
            transform.rotation = new Quaternion(0, mainCamera.transform.rotation.y, 0, mainCamera.transform.rotation.w);
            _isLeftTurning = false;
        }
        if (_isWalking)
        {
            if (!stack)
            {
                rb.AddRelativeForce(new Vector3(0.0f, 0.0f, plSpeed) * rb.mass, ForceMode.Impulse);
            }
            transform.rotation = new Quaternion(0, mainCamera.transform.rotation.y, 0, mainCamera.transform.rotation.w);

            _isWalking = false;
        }

        if (_isStepingBack)
        {
            rb.AddRelativeForce(new Vector3(0.0f, 0.0f, plSpeed) * rb.mass, ForceMode.Impulse);
            transform.rotation = new Quaternion(0, mainCamera.transform.rotation.y, 0, mainCamera.transform.rotation.w);
            _isStepingBack = false;
        }
        
        if (_isJumped)
        {
            if (jumpCounter > 0)
            {
                rb.velocity = Vector3.zero;
                rb.AddRelativeForce(jumpForce, ForceMode.Impulse);
                jumpCounter--;
            }
            _isJumped = false;
        }
    }

    //接地判定
    private void OnCollisionEnter(Collision collision)
    {
        floatingTimer = 0.0f;
        if (collision.contacts[0].normal.y > 0.70)
        {
            if (!_isGrounded&&_isJumping)
            {
                _isJumping = false;
                animator.SetFloat("jump", 0.0f);
                animator.SetBool("isJumping", false);
            }
            animator.SetBool("isGrounded", true);
            _isGrounded = true;

            if (animator.GetBool("isGrounded")&& collision.gameObject.tag != "Untagged")
            {
                if (fieldManager.transform.Find(collision.gameObject.tag + "Manager") != null)
                {
                    MonoBehaviour targetScript = fieldManager.transform.Find(collision.gameObject.tag + "Manager").GetComponent<MonoBehaviour>();
                    MethodInfo method = targetScript.GetType().GetMethod("PlayerEnter");
                    method.Invoke(targetScript, new object[] { true });

                    MethodInfo method2 = targetScript.GetType().GetMethod("EnterField");
                    method2.Invoke(targetScript, new object[] { collision.gameObject });
                }
            }
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

            if (animator.GetBool("isGrounded") && collision.gameObject.tag != "Untagged")
            {
                if (fieldManager.transform.Find(collision.gameObject.tag + "Manager") != null)
                {
                    MonoBehaviour targetScript = fieldManager.transform.Find(collision.gameObject.tag + "Manager").GetComponent<MonoBehaviour>();
                    MethodInfo method = targetScript.GetType().GetMethod("PlayerStay");
                    method.Invoke(targetScript, new object[] { true });
                }
            }
        }

        if (_isGrounded && jumpCounter!= jumpLimit)
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
        if (stack) { stack = false; }

        if (animator.GetBool("isGrounded") && collision.gameObject.tag != "Untagged")
        {
            if (fieldManager.transform.Find(collision.gameObject.tag + "Manager") != null)
            {
                MonoBehaviour targetScript = fieldManager.transform.Find(collision.gameObject.tag + "Manager").GetComponent<MonoBehaviour>();
                MethodInfo method = targetScript.GetType().GetMethod("PlayerExit");
                method.Invoke(targetScript, new object[] { true });
            }
        }
    }
}