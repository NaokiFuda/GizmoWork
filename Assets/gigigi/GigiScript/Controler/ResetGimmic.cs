
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ResetGimmic : UdonSharpBehaviour
{
    [SerializeField] private GameObject[] ResetPoss;
    [SerializeField] GameObject[] Wipes;
    [SerializeField] GameObject[] Pops;
    [SerializeField] GameObject[] Anims;
    private Vector3[] Poss = new Vector3[100];
    private Quaternion[] Rots = new Quaternion[100];
    private int i = 0;
    private int SignalNum1 = 0;
    private int SignalNum2 = 0;
    private int SignalNum3 = 0;
    private int SignalNum4 = 0;

    Animator animator;

    void Start()
    {
        SignalNum1 = ResetPoss.Length;
        for (i = 0; i < SignalNum1; i++)
        {
            Poss[i] = ResetPoss[i].gameObject.transform.position;
            Rots[i] = ResetPoss[i].gameObject.transform.rotation;
        }
        SignalNum2 = Wipes.Length;
        SignalNum3 = Pops.Length;
        SignalNum4 = Anims.Length;
    }
    public override void Interact()
    {
        if (SignalNum1 != 0)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "ResetPos");
        }
        if (SignalNum2 != 0)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "GimWipe");
        }
        if (SignalNum3 != 0)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "GimPop");
        }
        if (SignalNum4 != 0)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "Deact");
        }
    }
    public void ResetPos()
    {
        for (i = 0; i < SignalNum1; i++)
        {
            ResetPoss[i].gameObject.transform.position = Poss[i];
            ResetPoss[i].gameObject.transform.rotation = Rots[i];
        }
    }
    public void GimWipe()
    {
        for (i = 0; i < SignalNum2; i++)
        {
            Wipes[i].gameObject.SetActive(false);
        }
    }
    public void GimPop()
    {
        for (i = 0; i < SignalNum3; i++)
        {
            Pops[i].gameObject.SetActive(true);
        }
    }
    public void Deact()
    {
        for (i = 0; i < SignalNum4; i++)
        {
            animator = Anims[i].GetComponent<Animator>();
            animator.SetBool("active", false);
        }
    }
}
