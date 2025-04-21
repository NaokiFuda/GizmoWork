
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OnPlayerTriggerStaySender : UdonSharpBehaviour
{
    [SerializeField] PlayerMovementSetter playerMovementSetter;

    BoxCollider hitCollider;
    float colliderHeight;
    float borderHeight;
    private void Start()
    {
        hitCollider = GetComponent<BoxCollider>();
        colliderHeight = transform.position.y + hitCollider.center.y + hitCollider.size.y / 2;
    }
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if(player.isLocal)
        {
            playerMovementSetter.OnPlayerStay(player);
            borderHeight = colliderHeight - player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position.y;
            playerMovementSetter.SetProgramVariable("borderHeight", borderHeight);
        }
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            playerMovementSetter.OnPlayerEnter(player);
        }
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            playerMovementSetter.OnPlayerExit(player);
        }
    }
}
