
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FollowPlayerOnHead : UdonSharpBehaviour
{
    [SerializeField] bool lookAtPlayer = true;
    public void Activate(Transform target)
    {
        transform.parent = target;
        transform.localPosition = Vector3.up;
    }

    void Update()
    {
        if (lookAtPlayer)
        {
            Vector3 directionToPlayer = transform.position - Networking.LocalPlayer.GetPosition();
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = targetRotation;
            }
        }
    }
}
