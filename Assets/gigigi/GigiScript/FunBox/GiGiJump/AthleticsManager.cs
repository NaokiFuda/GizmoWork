﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AthleticsManager : UdonSharpBehaviour
{
    Rigidbody[] verLiftRBs;
    Rigidbody[] horiLiftRBs;
    [SerializeField] private GameObject[] verticalLifts;
    [SerializeField] private GameObject[] horizontalLifts;

    [SerializeField] private Vector3 _leftWave = Vector3.left * 2.0f;
    [SerializeField] private Vector3 _rightWave = Vector3.right * 2.0f;
    [SerializeField] private float _returnTime = 3.0f;

    [SerializeField] private Vector3 _upWave = Vector3.up * 2.0f;
    [SerializeField] private Vector3 _downWave = Vector3.down * 2.0f;

    private Vector3 lastVelocity;

    private bool liftUpDown = false;

    private float currentTime;
    private float playerOffTimer = 0.0f;

    //他のスクリプトから値を変更する奴。いわゆるsetter。「unity get set」とかでググると出てくるよ
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
        //配列にリフトの一覧にあるオブジェクトの情報を入れて一括で操作する準備をしてるよ。○○.Lengthは配列に入ってる要素の数だよ。配列は要素を入れる前に要素を何個入れる予定か決める必要があるよ。
        verLiftRBs = new Rigidbody[verticalLifts.Length];
        horiLiftRBs = new Rigidbody[horizontalLifts.Length];
        for (i = 0; i < verticalLifts.Length; i++)
        {
            verLiftRBs[i] = verticalLifts[i].GetComponent<Rigidbody>();
        }
        for (i = 0; i < horizontalLifts.Length; i++)
        {
            horiLiftRBs[i] = horizontalLifts[i].GetComponent<Rigidbody>();
        }
    }
    void Update()
    {
        if (!playerEnter) { return; }//プレイヤーが乗ってなかったら動かないようにするよ
        playerOffTimer += Time.deltaTime;
        //プレイヤーがリフトに乗ってない状態が0.8f続いたら降りたことにするよ。または、リフトの慣性に逆らう（ジャンプ操作中に空中制御したら）なら降りたことにするよ
        if (playerOffTimer > 0.8f && Networking.LocalPlayer.IsPlayerGrounded() && !playerStay || Vector3.Dot(Networking.LocalPlayer.GetVelocity(), lastVelocity) < -0.5f && !Networking.LocalPlayer.IsPlayerGrounded())
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
        //リフトがひとつも登録されてないならリフトは動かないよ。
        if (verticalLifts.Length == 0 && horizontalLifts.Length == 0) { return; }
        currentTime += Time.deltaTime;
        if (currentTime > _returnTime)
        {
            for (i = 0; i < verLiftRBs.Length; i++)
            {
                verLiftRBs[i].velocity = _downWave;
            }
            for (i = 0; i < horiLiftRBs.Length; i++)
            {
                horiLiftRBs[i].velocity = _leftWave;
            }

            if (currentTime > _returnTime * 2)
            {
                currentTime = 0.0f;
            }
        }
        else
        {
            for (i = 0; i < verLiftRBs.Length; i++)
            {
                verLiftRBs[i].velocity = _upWave;
            }
            for (i = 0; i < horiLiftRBs.Length; i++)
            {
                horiLiftRBs[i].velocity = _rightWave;
            }
        }
        if (playerEnter)
        {
            //メソッドを実行するよ
            FrictionWork();
        }
    }
    public void FrictionWork()
    {
        //配列verticalLiftsの中にenterFieldに代入されているGameObjectはあるか？ないなら-1あるなら0から始まるindex番号を出す
        if (verticalLifts.Length != 0)
        {
            for (i=0; i< verticalLifts.Length; i++)
            {
                if (verticalLifts[i] == enterField)
                {
                    liftUpDown = false;
                }
                else
                {
                    liftUpDown = true;
                }
            }
        }
        if (Networking.LocalPlayer.IsPlayerGrounded())
        {
            if (liftUpDown)
            {
                Networking.LocalPlayer.SetVelocity(verLiftRBs[0].velocity);
                lastVelocity = verLiftRBs[0].velocity;
            }
            else
            {
                //リフトに乗ってる間、着地してるとリフトの動きとプレイヤーの動きを同期するよ
                Networking.LocalPlayer.SetVelocity(horiLiftRBs[0].velocity);
                lastVelocity = horiLiftRBs[0].velocity;
            }
        }
        if (!Networking.LocalPlayer.IsPlayerGrounded())
        {
            if (!liftUpDown)
            {
                //リフトの上でジャンプするとリフトの動きを疑似的な慣性としてジャンプのベクトルに乗せるよ。
                if (lastVelocity != horiLiftRBs[0].velocity)
                {
                    Networking.LocalPlayer.SetVelocity(lastVelocity - new Vector3(playerOffTimer, 0.0f, playerOffTimer) + Networking.LocalPlayer.GetVelocity());
                }
                else
                {
                    Networking.LocalPlayer.SetVelocity(horiLiftRBs[0].velocity - new Vector3(playerOffTimer, 0.0f, playerOffTimer) + Networking.LocalPlayer.GetVelocity());
                }
            }
            else
            {
                if (lastVelocity != verLiftRBs[0].velocity)
                {
                    Networking.LocalPlayer.SetVelocity(lastVelocity * 0.05f);
                }
                else
                {
                    Networking.LocalPlayer.SetVelocity(verLiftRBs[0].velocity * 0.05f);
                }
            }
        }
    }

}
