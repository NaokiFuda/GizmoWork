
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static UnityEngine.UI.Image;

public class PlayerRayManager : UdonSharpBehaviour
{
    Ray playerRay;

    public Ray GetPlayerRay() {  return playerRay; }

     VRCPlayerApi.TrackingData playerHead;
    public VRCPlayerApi.TrackingData GetPlayerHead() { return playerHead; }

    public Transform GetPlayerHeadTransform() { return transform; }
    public Vector3 GetPlayerHeadDirection(Vector3 direction)
    {
        if (direction == Vector3.up) return transform.up;
        else if (direction == Vector3.right) return transform.right;
        else if (direction == Vector3.left) return -transform.right;
        else if(direction == Vector3.forward) return transform.forward;
        else return Quaternion.FromToRotation(transform.forward, direction) * transform.forward;
    }

    private void Update()
    {
        playerHead = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        transform.rotation = playerHead.rotation;
        transform.position = playerHead.position;
        playerRay = new Ray(playerHead.position, transform.forward);
        Debug.DrawRay(playerHead.position, transform.forward * 1, Color.blue);
    }

}
