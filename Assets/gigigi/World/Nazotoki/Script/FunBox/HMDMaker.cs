
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HMDMaker : UdonSharpBehaviour
{
    [SerializeField] Transform otherLenz;
    [SerializeField] Transform head;
    void Start()
    {
        otherLenz.parent = transform;
    }
    bool _attached;
    Vector3 _offset;
    Quaternion _rot;
    public override void OnPickupUseDown()
    {
        GetComponent<VRC_Pickup>().Drop();
        _offset = Quaternion.Inverse(head.rotation) * (transform.position - head.position);
        _rot = Quaternion.Inverse(head.rotation) * transform.rotation;
        _attached = true;
        
    }
    public override void OnPickup()
    {
        _attached = false;
    }
    private void Update()
    {
        if(_attached)
        {
            
            transform.rotation = head.rotation * _rot;
            transform.position = head.position + head.rotation * _offset;
        }
    }
}
