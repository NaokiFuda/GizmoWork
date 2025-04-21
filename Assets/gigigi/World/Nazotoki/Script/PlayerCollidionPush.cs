
using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerCollidionPush : UdonSharpBehaviour
{
    [SerializeField] float pullDegree = 1f;
    [SerializeField] bool xAxis;
    [SerializeField] bool yAxis;
    [SerializeField] Transform rotateRoot;
    [SerializeField] float rotateLimit = 90;
    [SerializeField] Transform hitTransform;
    Vector3 defDir;
    Vector3 lateTransform;

    private void Start()
    {
        if (rotateRoot == null) rotateRoot = transform;
        if (hitTransform == null) hitTransform = transform;
        defDir = rotateRoot.up;
        if (xAxis) defDir = rotateRoot.forward;
        else if (yAxis) defDir = rotateRoot.right;
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 26 )
        {
            if (other.transform.position == lateTransform) return;
            Vector3 hitPos = hitTransform.position;
            Vector3 axis = rotateRoot.forward;
            Vector3 lateAxis = rotateRoot.up;
            if (xAxis)
            {
                axis = rotateRoot.right;
                lateAxis = rotateRoot.forward;
            }
            else if (yAxis)
            {
                axis = rotateRoot.up;
                lateAxis = rotateRoot.right;
            }

            float additionalPow = 0;
            
            
            float distanceFromCenter = 0;
            if (xAxis) distanceFromCenter = (other.bounds.center - hitPos).z;
            else if (yAxis) distanceFromCenter = (other.bounds.center - hitPos).x;
            else distanceFromCenter = (other.bounds.center - hitPos).y;
            

            float limitScale = Vector3.SignedAngle(defDir, lateAxis, axis);

            if (rotateLimit > limitScale && rotateLimit * -1f < limitScale)
            {
                if (distanceFromCenter > 0) pullDegree = Mathf.Abs(pullDegree) * -1f;
                else pullDegree = Mathf.Abs(pullDegree);
            }
            else if (rotateLimit <= limitScale)
            {
                pullDegree = Mathf.Abs(pullDegree) * -1f;
                additionalPow = -0.5f;
            }
            else
            {
                pullDegree = Mathf.Abs(pullDegree) * 1f;
                additionalPow = 0.5f;
            }
            rotateRoot.rotation *= Quaternion.AngleAxis(pullDegree + additionalPow, axis);
            lateTransform = other.transform.position;

        }
    }
}
