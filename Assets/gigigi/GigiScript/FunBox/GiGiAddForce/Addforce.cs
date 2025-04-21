
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Addforce : UdonSharpBehaviour
{
    private VRCPlayerApi m_owner;
    private Vector3 force = new Vector3(0, 0, 0);
    private Vector3 lastPos = new Vector3(0, 0, 0);
    private Vector3 lastHPos = new Vector3(0, 0, 0);

    private bool pick = false;
    private bool hold = false;
    private float H_length = 0;
    [SerializeField] private float Hand_disHit = 0.1f;
    [SerializeField] private float Hand_disSeparete = 1.5f;
    [SerializeField] private float ThrowSpeed = 3.0f;
    [SerializeField] private float ChatchSpeed = 5.0f;

    void Start()
    {
        lastPos = transform.position;
        Hand_disHit = Hand_disHit * Hand_disHit;
        Hand_disSeparete = Hand_disSeparete * Hand_disSeparete;
    }

    public override void Interact()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        m_owner = Networking.LocalPlayer;
        pick = true;

    }
    
    void LateUpdate()
    {
        if (pick == true)
        {
            
            Rigidbody rb = this.GetComponent<Rigidbody>();
            Vector3 Righty = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position - lastPos;
            H_length = Righty.sqrMagnitude;
            
            if (hold == true)
            {
               
                if (Vector3.Dot(lastHPos - m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position, lastPos - m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position)  > 0.3f)
                {
                        Vector3 Righty2 = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position - lastPos;
                        rb.AddForce(Righty2 * ThrowSpeed, ForceMode.Impulse);
                        pick = false;
                        hold = false;
                        return;
                }

                lastPos = transform.position;
                rb.AddForce(Righty);

                if (H_length <= Hand_disHit)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    lastHPos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
                }
                
                return;
            }

            if (H_length <= Hand_disHit)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                lastHPos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
                hold = true;
            }

            lastPos = transform.position;
            rb.AddForce(Righty * ChatchSpeed);

            if (H_length >= Hand_disSeparete)
            {
                pick = false;
                return;
            }
        }
       
        
    }
   
}