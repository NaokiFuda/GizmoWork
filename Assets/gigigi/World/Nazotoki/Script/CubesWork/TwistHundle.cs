
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class TwistHundle : UdonSharpBehaviour
{
    [SerializeField] Collider capCol;
    [SerializeField] float twistLimit = 3240f;
    [SerializeField] float moveLimit = 0.09f;
    Vector3 _fromCapDir;
    Vector3 _prevHandRelateDir;
    Vector3 _prevForward;
    float _rotateStopper;
    public float timeScale = 1.0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 26 && Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
        {
            if (other.name[0] != 'H') { return; }

            if(capCol == null) capCol = GetComponent<Collider>();
            
            var handTransform = other.transform;

            var twistAxis = transform.forward;
            var currentForward = handTransform.forward;
            if (Vector3.Dot(currentForward, twistAxis) < -0.9f) currentForward = handTransform.right;

            float twistAngle = 0f;
            float compareDot = Vector3.Dot(currentForward, _prevForward);
            if (compareDot < 0.999f)
            {
                twistAngle = Vector3.SignedAngle(_prevForward, currentForward, twistAxis);
            }

            var capRot = Quaternion.FromToRotation(_fromCapDir, transform.up);
            var handRelatePos = capRot * _prevHandRelateDir;
            Vector3 pudhDir = handTransform.position - capCol.bounds.center;

            if (Vector3.Dot(handRelatePos, pudhDir) < 0f)
            {
                Debug.Log(Vector3.Dot(handRelatePos, pudhDir));
                OnPostInput(twistAxis, handTransform);
                return;
            }
            if(twistAngle == 0)
                twistAngle = Vector3.SignedAngle(handRelatePos, pudhDir, twistAxis);
            else
                twistAngle += Vector3.SignedAngle(handRelatePos, pudhDir, twistAxis);

            if (_rotateStopper + twistAngle > twistLimit)
            {
                OnPostInput(twistAxis, handTransform);
                return;
            }

            _rotateStopper += twistAngle;

            if (_rotateStopper + twistAngle < 0)
            {
                _rotateStopper = 0;
                transform.localRotation = Quaternion.identity;
                MoveWithRotate();
                OnPostInput(twistAxis, handTransform);
                return;
            }

            transform.rotation = Quaternion.AngleAxis(twistAngle, twistAxis) * transform.rotation;
            MoveWithRotate();

            OnPostInput( twistAxis, handTransform);
        }

    }
    private void OnPostInput(Vector3 twistAxis, Transform handTransform)
    {
        _fromCapDir = transform.up;
        _prevHandRelateDir = handTransform.position - capCol.bounds.center;
        _prevForward = handTransform.forward;
        if (Vector3.Dot(_prevForward, twistAxis) < -0.95f) _prevForward = handTransform.right;
    }
    void MoveWithRotate()
    {
            transform.localPosition = Vector3.forward * moveLimit * _rotateStopper / twistLimit;
    }
}
