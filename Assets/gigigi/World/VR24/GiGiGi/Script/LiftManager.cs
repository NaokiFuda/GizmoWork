using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftManager : MonoBehaviour
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
    
    private bool liftUpDown = false;

    private float currentTime;
    private float playerOffTimer = 0.0f;

    //���̃X�N���v�g����l��ύX����z�B������setter�B�uunity get set�v�Ƃ��ŃO�O��Əo�Ă����
    private GameObject enterField;
    public GameObject EnterField(GameObject value)
    {
        enterField = value;
        return enterField;
    }

    private bool playerEnter = false;
    public void PlayerEnter(bool value)
    {
        playerEnter = value;
    }

    private bool playerStay = false;
    public void PlayerStay(bool value)
    {
        playerStay = value;
    }
    private bool playerExit = false;
    public void PlayerExit(bool value)
    {
        playerExit = value;
    }

    private int i;

    void Start()
    {
        //�z��Ƀ��t�g�̈ꗗ�ɂ���I�u�W�F�N�g�̏������Ĉꊇ�ő��삷�鏀�������Ă��B����.Length�͔z��ɓ����Ă�v�f�̐�����B�z��͗v�f������O�ɗv�f���������\�肩���߂�K�v�������B
        verRiftRBs= new Rigidbody[verticalLifts.Length];
        horiRiftRBs = new Rigidbody[horizontalLifts.Length];
        for (i=0; i< verticalLifts.Length; i++)
        {
            verRiftRBs[i] = verticalLifts[i].GetComponent<Rigidbody>();
        }
        for (i = 0; i < horizontalLifts.Length; i++)
        {
            horiRiftRBs[i] = horizontalLifts[i].GetComponent<Rigidbody>();
        }
        //�v���C���[�̏�Ԃ��ώ@���邽�߂̏�������
        playerRB = player.GetComponent<Rigidbody>();
        animator = player.GetComponent<Animator>();
    }
    void Update()
    {
        if (!playerEnter) { return; }//�v���C���[������ĂȂ������瓮���Ȃ��悤�ɂ����
        playerOffTimer += Time.deltaTime;
        //�v���C���[�����t�g�ɏ���ĂȂ���Ԃ�0.8f��������~�肽���Ƃɂ����B�܂��́A���t�g�̊����ɋt�炤�i�W�����v���쒆�ɋ󒆐��䂵����j�Ȃ�~�肽���Ƃɂ����
        if (playerOffTimer > 0.8f && animator.GetBool("isGrounded") && !playerStay || Vector3.Dot(playerRB.velocity, lastVelocity) < -0.5f && !animator.GetBool("isGrounded"))
        {
            playerEnter = false;
            playerOffTimer = 0.0f;
        }
        if (playerStay)
        {
            playerOffTimer = 0.0f;
            playerStay = false;
        }
        
    }
    void FixedUpdate()
    {
        //���t�g���ЂƂ��o�^����ĂȂ��Ȃ烊�t�g�͓����Ȃ���B
        if (verticalLifts.Length == 0 && horizontalLifts.Length == 0) { return; }
        currentTime += Time.deltaTime;
        if (currentTime > _returnTime)
        {
            for (i=0;i< verRiftRBs.Length; i++)
            {
                verRiftRBs[i].velocity = _downWave;
            }
            for (i = 0; i < horiRiftRBs.Length; i++)
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
            for (i = 0; i < verRiftRBs.Length; i++)
            {
                verRiftRBs[i].velocity = _upWave;
            }
            for (i = 0; i < horiRiftRBs.Length; i++)
            {
                horiRiftRBs[i].velocity = _rightWave;
            }
        }
        if (playerEnter)
        {
            //���\�b�h�����s�����
            FrictionWork();
        }
    }
    public void FrictionWork()
    {
        //�z��verticalLifts�̒���enterField�ɑ������Ă���GameObject�͂��邩�H�Ȃ��Ȃ�-1����Ȃ�0����n�܂�index�ԍ����o��
        if (Array.IndexOf(verticalLifts, enterField) == -1)
        {
            liftUpDown = false;
        }
        else
        {
            liftUpDown = true;
        }
        if (animator.GetBool("isGrounded"))
        {
            if (liftUpDown)
            {
                playerRB.velocity = verRiftRBs[0].velocity;
                lastVelocity = verRiftRBs[0].velocity;
            }
            else
            {
                //���t�g�ɏ���Ă�ԁA���n���Ă�ƃ��t�g�̓����ƃv���C���[�̓����𓯊������
                playerRB.velocity = horiRiftRBs[0].velocity;
                lastVelocity = horiRiftRBs[0].velocity;
            }
        }
        if (!animator.GetBool("isGrounded"))
        {
            if (!liftUpDown)
            {
                //���t�g�̏�ŃW�����v����ƃ��t�g�̓������^���I�Ȋ����Ƃ��ăW�����v�̃x�N�g���ɏ悹���B
                if (lastVelocity != horiRiftRBs[0].velocity)
                {
                    playerRB.velocity += lastVelocity - new Vector3(playerOffTimer, 0.0f, playerOffTimer);
                }
                else
                {
                    playerRB.velocity += horiRiftRBs[0].velocity - new Vector3(playerOffTimer, 0.0f, playerOffTimer);
                }
            }
            else
            {
                if (lastVelocity != verRiftRBs[0].velocity)
                {
                    playerRB.velocity += lastVelocity * 0.05f;
                }
                else
                {
                    playerRB.velocity += verRiftRBs[0].velocity * 0.05f;
                }
            }
        }
    }
}
