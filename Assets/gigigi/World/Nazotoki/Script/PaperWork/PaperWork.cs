using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PaperWork : UdonSharpBehaviour
{
    FuwaFuwaMovement[] paperFlatters;
    FuwaFuwaMovement onFiledPaperClass;
    Vector3[] paperLocalPoss;
    Quaternion[] paperLocalRots;
    Rigidbody rb;
    private void Start()
    {
        paperFlatters = GetComponentsInChildren<FuwaFuwaMovement>();
        paperLocalPoss = new Vector3[paperFlatters.Length];
        paperLocalRots = new Quaternion[paperFlatters.Length];
        for (int i = 0; i < paperFlatters.Length; i++)
        {
            paperLocalPoss[i] = paperFlatters[i].transform.localPosition;
            paperLocalRots[i] = paperFlatters[i].transform.localRotation;
        }
    }
   
    public void EmitPaper()
    {
        _emitted = true;
        foreach (FuwaFuwaMovement t in paperFlatters) 
        {
            rb = t.transform.GetComponent<Rigidbody>();
            t.transform.parent = null;
            rb.useGravity = true;
            rb.isKinematic = false;
            t.GetComponent<VRC_Pickup>().pickupable = true;
            t.GetComponent<Collider>().enabled = true;
        }
    }
    public void ResetPaper(Transform t, FuwaFuwaMovement method)
    {
        rb = t.transform.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        if (t.GetComponent<VRC_Pickup>().IsHeld) t.GetComponent<VRC_Pickup>().Drop();
        t.parent = transform;
        for (int i = 0; i < paperFlatters.Length; i++)
        {
           if(paperFlatters[i].gameObject == method.gameObject)
            {
                t.localPosition = paperLocalPoss[i];
                t.localRotation = paperLocalRots[i];
            }
        }
        t.GetComponent<VRC_Pickup>().pickupable = false;
        t.GetComponent<Collider>().enabled = false;
        method.DoStraighten();
    }

    [SerializeField] FitSightAndFade sendSignal;
    bool _emitted;
    public override void OnPickupUseDown()
    {
        if (!sendSignal.GetEffectWorked())
        {
            if (Networking.GetOwner(sendSignal.gameObject) != Networking.LocalPlayer) Networking.SetOwner(Networking.LocalPlayer, sendSignal.gameObject);
            sendSignal.SendCustomEvent("Activate");
        }
        
        if(!_emitted) EmitPaper();
    }

    public override void InputDrop(bool value, UdonInputEventArgs args)
    {
        if(_colliderIn && value && onFiledPaperClass != null)
        {
            ResetPaper(onFiledPaperClass.transform, onFiledPaperClass);
            _emitted = false;
        }
    }
    bool _colliderIn;
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.GetComponent<FuwaFuwaMovement>())
        {
            _colliderIn = false;
            onFiledPaperClass = null;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<FuwaFuwaMovement>())
        {
            _colliderIn = true;
            onFiledPaperClass = collision.transform.GetComponent<FuwaFuwaMovement>();
        }
    }
}
