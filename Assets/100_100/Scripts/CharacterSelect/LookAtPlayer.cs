
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LookAtPlayer : UdonSharpBehaviour
{
    bool _isActive;
    private void Update()
    {
        if(_isActive)
        transform.rotation = Quaternion.identity;
    }
}
