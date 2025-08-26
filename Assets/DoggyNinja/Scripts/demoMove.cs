using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demoMove : MonoBehaviour
{
    [SerializeField] int state;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("state",state);
    }
    private void Update()
    {
        if(animator.GetInteger("state") == 1)
        {
            transform.position += Vector3.forward * 0.001f;
        }
    }

}
