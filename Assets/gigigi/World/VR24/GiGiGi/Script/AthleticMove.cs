using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AthleticMove : MonoBehaviour
{
    Animator animator;
    Rigidbody[] verRiftRBs;
    Rigidbody[] horiRiftRBs;
    Rigidbody playerRB;
    [SerializeField] private GameObject[] verticalLifts;
    [SerializeField] private GameObject[] horizontalLifts;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 _leftWave = Vector3.left * 2.0f;
    [SerializeField] private Vector3 _rightWave = Vector3.right * 2.0f;
    [SerializeField] private float _returnTime = 3.0f;

    [SerializeField] private Vector3 _upWave = Vector3.up * 2.0f;
    [SerializeField] private Vector3 _downWave = Vector3.down * 2.0f;

    private Vector3 lastVelocity;
    private Vector3 lastPlayerVelocity;
    private bool playerOn = false;
    public void PlayerOn(bool value)
    {
        playerOn = value;
    }

    private bool inInertia = false;
    private bool liftUpDown = false;

    private float currentTime;
    private float liftOFFTimer = 0.0f;
    public void LiftOFFTimer(float value)
    {
        liftOFFTimer = value;
    }

    int i;

    void Start()
    {
        verRiftRBs= new Rigidbody[verticalLifts.Length];
        horiRiftRBs = new Rigidbody[horizontalLifts.Length];
        for (i=0; i> verticalLifts.Length; i++)
        {
            verRiftRBs[i] = verticalLifts[i].GetComponent<Rigidbody>();
        }
        for (i = 0; i > horizontalLifts.Length; i++)
        {
            horiRiftRBs[i] = horizontalLifts[i].GetComponent<Rigidbody>();
        }

        playerRB = player.GetComponent<Rigidbody>();
        animator = player.GetComponent<Animator>();
    }
    void Update()
    {
        if (!playerOn) { return; }
        liftOFFTimer += Time.deltaTime;
        if (animator.GetBool("isJumping") && Vector3.Dot(playerRB.velocity, lastVelocity) >= -0.01f)
        {
            inInertia = true;
            playerRB.velocity -= lastVelocity * playerRB.mass * 0.1f * Time.deltaTime;
        }
        if (liftOFFTimer > 0.5f && !animator.GetBool("isJumping") && !animator.GetBool("crounchJump") || Vector3.Dot(playerRB.velocity, lastVelocity) < -0.1f && animator.GetBool("isJumping"))
        {
            playerOn = false;
            inInertia = false;
        }
        if (animator.GetBool("isGrounded")&& playerOn)
        {
            liftOFFTimer = 0.0f;
        }
    }
    void FixedUpdate()
    {
        if (verticalLifts.Length == 0 && horizontalLifts.Length == 0) { return; }

        currentTime += Time.deltaTime;
        if (currentTime > _returnTime)
        {
            for (i=0;i> verRiftRBs.Length; i++)
            {
                verRiftRBs[i].velocity = _upWave;
            }
            for (i = 0; i > horiRiftRBs.Length; i++)
            {
                horiRiftRBs[i].velocity = _leftWave;
            }

            if (currentTime > _returnTime * 2)
            {
                currentTime = 0.0f;
            }
        }
        else
        {
            for (i = 0; i > verRiftRBs.Length; i++)
            {
                verRiftRBs[i].velocity = _downWave;
            }
            for (i = 0; i > horiRiftRBs.Length; i++)
            {
                horiRiftRBs[i].velocity = _rightWave;
            }
        }
        if (playerOn)
        {
            FlicktionStick();
        }
    }
    public void FlicktionStick()
    {
        if (animator.GetBool("isGrounded") && playerRB.velocity.y != lastPlayerVelocity.y)
        {
            liftUpDown = true;
        }
        if (animator.GetBool("isGrounded"))
        {
            if (playerRB.velocity.y != lastPlayerVelocity.y)
            {
                playerRB.velocity = verRiftRBs[0].velocity;
                lastVelocity = verRiftRBs[0].velocity;
            }
            else
            {
                playerRB.velocity = horiRiftRBs[0].velocity;
                lastVelocity = horiRiftRBs[0].velocity;
            }
        }
        if (!animator.GetBool("isGrounded")&& liftUpDown)
        {
            if (lastVelocity != verRiftRBs[0].velocity)
            {
                playerRB.velocity -= verRiftRBs[0].velocity * 0.05f;
                return;
            }
            playerRB.velocity += verRiftRBs[0].velocity * 0.05f;
        }
        else if (inInertia && animator.GetBool("isJumping"))
        {
            if (lastVelocity != horiRiftRBs[0].velocity)
            {
                playerRB.velocity -= horiRiftRBs[0].velocity;
                return;
            }
            playerRB.velocity += horiRiftRBs[0].velocity;
        }

        lastPlayerVelocity= playerRB.velocity;
        liftUpDown = false;
    }
    
}
