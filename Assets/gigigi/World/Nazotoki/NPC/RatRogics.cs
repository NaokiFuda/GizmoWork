
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RatRogics : UdonSharpBehaviour
{
    [SerializeField] float speed = 0.05f;
    [SerializeField] int targetID = 26;
    [SerializeField] Transform ratHead;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] float ratSightRange = 2.5f;
    [SerializeField] float stayRange = 1.5f;
    [SerializeField] float returnTime= 30f;
    bool _following;
    bool _idling;
    float _timer;
    Transform _target;
    Vector3 _defPos;
    Quaternion _defRot;

    private void Start()
    {
        _defPos = transform.position;
        _defRot = transform.rotation;
    }
    private void Update()
    {
        if (!_following || _idling)
        {
            Ray ray = new Ray(ratHead.position, ratHead.forward);
            RaycastHit[] hits = Physics.SphereCastAll(ray, 1f, ratSightRange);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.layer == targetID)
                {
                    animator.SetBool("follow", true);
                    _following = true;
                    animator.SetBool("idle", false);
                    _idling = false;
                    _target = hit.transform;
                    break;
                }
            }
        }
        if(_following && !_idling)
        {
            _timer = 0;
            if (Vector3.Distance(_target.position,transform.position) < stayRange)
            {
                animator.SetBool("idle", true);
                _idling = true;
            }
        }
        if(_idling)
        {
            _timer += Time.deltaTime;
            if(_timer > returnTime/2)
            {
                _following = false;
                animator.SetBool("follow", false);
            }
            if(_timer > returnTime)
            {
                transform.position = _defPos;
                transform.rotation = _defRot;
                _timer = 0;
            }
        }
        
    }
    Vector3 _lastPos;
    float _stackTimer;
    bool _jump;
    float _jumpTimer;
    private void FixedUpdate()
    {
        if (_following && !_idling) 
        {
            //animator.SetFloat("lookAngle", lookAngle);
            Quaternion targetRot = Quaternion.LookRotation(ratHead.rotation * ratHead.forward);
            targetRot.x = 0f; targetRot.z = 0f;
            Debug.Log(targetRot);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 0.05f);
            Vector3 force = transform.forward * speed * rb.mass / Time.fixedDeltaTime;
            force.y = 0;
            rb.AddForce(force);
            float movingDegree = (transform.position - _lastPos).sqrMagnitude;
            if (movingDegree < 0.00000001f)
            {
                _stackTimer += Time.deltaTime;
                if (_stackTimer > 3.0f)
                {
                    _jump = true;
                    _stackTimer = 0;
                }
                
            }
            
            if (_jump) 
            {
                _jumpTimer += Time.deltaTime;
                rb.velocity = Vector3.up * Mathf.Lerp(rb.velocity.y,10f,0.1f);
                if (_jumpTimer > 0.3f)
                {
                   _jump = false;
                    _jumpTimer = 0;
                }
            }
            if (Mathf.Abs((transform.position - _lastPos).y) > 0.001f)
            {
                rb.velocity += Vector3.up * Mathf.Lerp(rb.velocity.y, -40f, 0.01f);
            }

                _lastPos = transform.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 4)
        {
            _idling = true;
            animator.SetBool("idle", true);
        }
    }
}
