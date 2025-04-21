
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiGravityTool : UdonSharpBehaviour
{
    [SerializeField] private Vector3 localGravity;
    private Rigidbody rBody;
    private Vector3 a;

    private void Start()
    {
        rBody = this.GetComponent<Rigidbody>();
        rBody.useGravity = false;
        a = localGravity;
    }
    private void FixedUpdate ()
    {
        SetLocalGravity();
    }
    private void SetLocalGravity()
    {
        rBody.AddForce(localGravity, ForceMode.Acceleration);
    }
    public void Activate()
    {
        localGravity = new Vector3(0,0,0);
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }
    public void DeActivate()
    {
        localGravity = a;
    }
}
