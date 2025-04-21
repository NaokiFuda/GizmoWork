using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketParry : MonoBehaviour
{
    [SerializeField] GameObject[] bullets;
    Rigidbody[] rbs;
    private bool _launched;
    private bool _reflected;
    int i;
    void Start()
    {
        
    }

    void Update()
    {
        if (_launched)
        {
            bullets = GameObject.FindGameObjectsWithTag("Bullet");
            for (i = 0; i > bullets.Length; i++)
            {
                rbs[i] = bullets[i].GetComponent<Rigidbody>();
            }
        }
    }
    private void FixedUpdate()
    {
        //‘SŽè“®’e“¹ŒvŽZver
        /*
        if (_launched)
        {

        }
        */
        //‘SŽ©“®’e“¹ŒvŽZver
        if (_launched)
        {

        }
    }
    void OnTriggerEnter(Collider other)
    {
        for (i = 0; i < bullets.Length; i++)
        {
            if (bullets[i] == other.gameObject)
            {
                bullets[i].gameObject.SetActive(false);
            }
        }
    }

  
}
