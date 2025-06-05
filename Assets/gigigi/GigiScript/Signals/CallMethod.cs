
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class CallMethod : UdonSharpBehaviour
{
    [SerializeField] string[] methodNames = new string[1] {"Activate"};

    [SerializeField] int[] udonSlotIndex = new int[1] ;
    [SerializeField] GameObject[] targets;
    UdonBehaviour[] signals;
    [SerializeField] bool isLocal;
    [SerializeField] bool isPushButton;
    [SerializeField] float pressThreshold = 0.1f;

    Collider buttonCollider;
    Vector3 _defPos;
    bool _released;
    [SerializeField] bool allowHold;
    [SerializeField] bool allow;
    private void Start()
    {
        _defPos = transform.localPosition;
        signals = new UdonBehaviour[targets.Length];
        if (isPushButton) buttonCollider = GetComponent<Collider>();
        for (int i = 0; i < targets.Length; i++)
        {
            var u = targets[i].GetComponents<UdonBehaviour>();
            signals[i] = u[udonSlotIndex[i]];
        }
        
    }
    public override void Interact()
    {
        Activate();
    }

    void Activate()
    {
        for (int i = 0; i < signals.Length; i++)
        {
                signals[i].SendCustomEvent(methodNames[i]);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ( other.gameObject.layer != 26)return;

        VRCPlayerApi player = Networking.GetOwner(other.gameObject);
        if (player.isLocal && isPushButton)
        {
            if (!_released && allowHold) _released = true;

            Vector3 rightpos = player.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            Vector3 leftpos = player.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
            if (buttonCollider.bounds.Contains(rightpos))
            {
                var pos = new Vector3(rightpos.x * transform.position.x, rightpos.y * transform.position.y, rightpos.z * transform.position.z);
                PressButtom( Vector3.Distance(pos , buttonCollider.bounds.center) );
            }
            if (buttonCollider.bounds.Contains(leftpos))
            {
                var pos = new Vector3(leftpos.x * transform.position.x, leftpos.y * transform.position.y, leftpos.z * transform.position.z);
                PressButtom(Vector3.Distance(pos, buttonCollider.bounds.center));
            }
        }
    }
    void PressButtom(in float pressAmount)
    {
        if (_released && _defPos.y - transform.localPosition.y >= pressThreshold)
        {
            Activate();
            _released = false;
        }
        if(_defPos.y - transform.localPosition.y <= buttonCollider.bounds.size.y / 2)
        transform.localPosition -= transform.up * Mathf.Clamp(pressAmount, 0, buttonCollider.bounds.size.y / 2);
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 26) return;

        if (isPushButton)
        {
            if(transform.localPosition.y < _defPos.y)  _released = true;
        }
            
    }
    private void Update()
    {
        if(_released && transform.localPosition.y < _defPos.y) transform.localPosition += transform.up * 0.1f;
    }
}
