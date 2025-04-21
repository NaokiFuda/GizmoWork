
using System;
using UdonSharp;
using UnityEngine;
using VRC.Core;
using VRC.SDK3.ClientSim.PlayerTracking;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RadioSwitch : UdonSharpBehaviour
{
    [SerializeField] UdonBehaviour[] targets;
    [SerializeField] String ButtonName;
    [SerializeField] String methodName;
    [SerializeField] bool autoReset;
    [SerializeField] bool longPush;
    [SerializeField] bool isLocal;
    [SerializeField] Animator animator;
    [SerializeField, Tooltip("コライダーサイズを１として押し込める割合"), Range(0,1)] float pushingDegree = 0.01f;
    [SerializeField] float rockDegree = 0.8f;
    
    [SerializeField] float _timeInterval= 0.1f;
    Collider thisCollider;
    bool buttonOn;
    bool buttonState;

    Vector3 hitPos;
    float pushedDistance;
    void Start()
    {
        thisCollider = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 26)
        {
            hitPos = thisCollider.bounds.center + transform.up * thisCollider.bounds.max.y;
            pushedDistance = Vector3.Distance(hitPos, other.bounds.center - Vector3.up * other.bounds.min.y);
            if (pushedDistance > thisCollider.bounds.size.y * pushingDegree)
            {
                if (animator.GetFloat(ButtonName) >= rockDegree && !buttonOn)
                {
                    buttonOn = true;
                    buttonState = !buttonState;
                    if (longPush) SendLongSgnals();
                    else SendSgnals();
                }
                if (!buttonState || autoReset)
                    animator.SetFloat(ButtonName, Mathf.Clamp(pushedDistance / thisCollider.bounds.size.y / pushingDegree, 0, 1));
            }
        }
    }

    private void SendSgnals()
    {
        if (!isLocal)
        {
            if (targets.Length == 0) SendCustomNetworkEvent(NetworkEventTarget.All, methodName);
            else
            {
                foreach (UdonBehaviour target in targets) target.SendCustomNetworkEvent(NetworkEventTarget.All, methodName);
            }
        }
        else
        {
            if (targets.Length == 0) SendCustomEvent(methodName);
            else
            {
                foreach (UdonBehaviour target in targets) target.SendCustomEvent(methodName);
            }
        }
    }
    public void SendLongSgnals()
    {
        if (buttonOn) 
        {
            SendSgnals();
            if (_timeInterval > 0)
            {
                SendCustomEventDelayedSeconds(nameof(SendLongSgnals), _timeInterval);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        buttonOn = false;
        if (autoReset)
        {
            DoFuction();
        }
        else if(!buttonState) DoFuction();
    }
    public void DoFuction()
    {
        if (animator.GetFloat(ButtonName) > 0)
        {
            animator.SetFloat(ButtonName, -0.1f);
            if (animator.GetFloat(ButtonName) < 0) animator.SetFloat(ButtonName, 0);
            if (animator.GetFloat(ButtonName) == 0) return;
            if (_timeInterval > 0)
            {
                SendCustomEventDelayedSeconds(nameof(DoFuction), _timeInterval);
            }
        }
    }
}
