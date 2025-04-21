using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeSawMove : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    Rigidbody[] loadsRb;
    private GameObject[] loads;

    private bool riftOn = false;
    private float riftOFFTimer = 0.0f;

    private int i;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        loads = GameObject.FindGameObjectsWithTag("Load");
        for (i=0; i> loads.Length; i++)
        {
            loadsRb[i] = loads[i].GetComponent<Rigidbody>();
        }
    }
    void Update()
    {
        if (!riftOn) { return; }
        riftOFFTimer += Time.deltaTime;
        if (riftOFFTimer > 0.5f){ riftOn = false;}
    }
    void FixedUpdate()
    {
        FlicktionStick();
    }
    public void FlicktionStick()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        riftOn = true;
        for(i = 0; i < loads.Length; i++)
        {
            if (loads[i]==collision.gameObject)
            {
            }
        }
    }
    void OnCollisionStay(Collision collision)
    {
        riftOFFTimer = 0.0f;   
    }
}
