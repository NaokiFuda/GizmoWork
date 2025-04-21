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
            //GetChild(i)��i+1�Ԗڎq�v�f���w���B�q���̎q����GetChild().GetChild()�ƌJ��Ԃ��Ď���B
            fields[i] = transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        if (playerEnter)
        {
            //�n�`���ʂł���Ȃ�PlayOneShot�Ƃ��ő����Ƃ��炵�Ă�������ˁB
            if (Array.IndexOf(fields, enterField) != -1)
                Debug.Log(enterField.name + "�ɏ���Ă�");
            playerEnter = false;
        }
    }
}
