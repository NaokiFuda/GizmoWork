
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class EyeSightPointer : UdonSharpBehaviour
{
    [SerializeField] PlayerRayManager playerRayManager;
    [SerializeField] EyeExerciseManager eyeExerciseManager;
    [SerializeField] Transform startPoint;
    [SerializeField] float hitDistance;
    float _maxDistance;

    private void OnEnable()
    {
        if(hitDistance ==0)
            _maxDistance = Vector3.Distance(startPoint.position, eyeExerciseManager.popArea.position);
        else 
            _maxDistance = hitDistance;
    }

    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        if(value)
        {
            Ray ray = playerRayManager.GetPlayerRay();
            RaycastHit hit;

                if (Physics.Raycast(ray, out hit , _maxDistance, 1 << 27) && hit.collider.gameObject.name.Contains("Target"))
                    eyeExerciseManager.KnockBackTarget(hit.collider.transform, args);
        }
        else
            eyeExerciseManager.UnLockTarget();
    }
}
