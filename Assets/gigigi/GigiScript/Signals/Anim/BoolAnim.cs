
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class BoolAnim : UdonSharpBehaviour
{
    [SerializeField] GameObject[] Signals;
    private int i = 0;
    private int SignalNum = 0;

    bool act = false;
    Animator animator;
    public bool IsLocal = false;

    void Start()
    {
        SignalNum = Signals.Length;
    }

    public override void Interact()
    {
        if (IsLocal == false)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
        }
        else 
        {
            SendCustomEvent("Activate");
        }
    }
    public void Activate()
    {
        i = Signals.Length;

        if (i == 0)
        {
            animator = this.gameObject.GetComponent<Animator>();
            act = animator.GetBool("active");
            animator.SetBool("active", !act);
        }
        for (i = 0; i < SignalNum; i++)
        {
            animator = Signals[i].GetComponent<Animator>();
            act = animator.GetBool("active");
            animator.SetBool("active", !act);
        }
    }
}
