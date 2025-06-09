
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class EyeOccluderSingnal : UdonSharpBehaviour
{
    [SerializeField] Transform eyeLevel;
    [SerializeField] VRC_Pickup pickup;
    [SerializeField] PlayerRayManager playerRayManager;
    [SerializeField] string methodName = "Active";
    [SerializeField] UdonBehaviour target;
    bool _isFocus;
    Vector3 _lastPos;
    [SerializeField] float threshold = 0.05f;
    [SerializeField] FitSightAndFade effectScript;
    float _timer ;
    void Update()
    {
        if (pickup != null && pickup.IsHeld && pickup.currentPlayer == Networking.LocalPlayer)
        {
            Ray ray = playerRayManager.GetPlayerRay();
            if (!_isFocus && Physics.Raycast(ray, out RaycastHit hit, 10f, 1 << 27) && hit.distance > 0.5f)
            {
                if (Networking.LocalPlayer.IsUserInVR() && Vector3.Distance(transform.position, Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position) >= 0.1f) return;
                _isFocus = true;
                _lastPos = transform.position;
                effectScript.Activate();
                _timer = 0;
                
            }

        }
        if(_isFocus && pickup.IsHeld)
        {
            _timer += Time.deltaTime;
            if (_timer < 1.5f) return;

            Vector3 dir = transform.position - _lastPos;
            if (dir.sqrMagnitude > 0.0001f)
            {
                float dot = Vector3.Dot(dir, Vector3.up);
                if (dot > threshold) 
                {
                    _isFocus = false;
                    pickup.Drop();
                    target.SendCustomEvent(methodName);
                }
            }
            _lastPos = transform.position;
        }
        
    }
    public override void InputDrop(bool value, UdonInputEventArgs args)
    {
        if(!value)_isFocus = false;
    }
}
