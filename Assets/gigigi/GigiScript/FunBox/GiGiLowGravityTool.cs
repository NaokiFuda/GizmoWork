
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiLowGravityTool : UdonSharpBehaviour
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            localGravity = new Vector3(0, 0, 0);
            localGravity = new Vector3(0, -0.5f, 0);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            localGravity = new Vector3(0, 0, 0);
            localGravity = a;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }

    void OnCollisionExit(Collision collision)
    {
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }

    public override void OnDrop()
    {
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        SetLocalGravity();
    }
    private void SetLocalGravity()
    {
        rBody.AddForce(localGravity, ForceMode.Acceleration);
    }
    public void StopFallen()
    {
        localGravity = new Vector3(0, 0, 0);
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }
    public void StartFallen()
    {
        localGravity = a;
    }
}
