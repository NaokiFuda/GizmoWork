using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManeger : MonoBehaviour
{
    private GameObject[] fields;
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
    void Start()
    {
        fields = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            //GetChild(i)はi+1番目子要素を指す。子供の子供はGetChild().GetChild()と繰り返して取れる。
            fields[i] = transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        if (playerEnter)
        {
            //地形判別できるならPlayOneShotとかで足音とか鳴らしてもいいよね。
            if (Array.IndexOf(fields, enterField) != -1)
                Debug.Log(enterField.name + "に乗ってる");
            playerEnter = false;
        }
    }
}
