
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PullCar : UdonSharpBehaviour
{
    [SerializeField] Transform pickObj;
    [SerializeField] Transform chainObj;

    private void Start()
    {
        if (pickObj != null) pickObj = transform;
    }
    public override void Interact()
    {
        
    }
}
