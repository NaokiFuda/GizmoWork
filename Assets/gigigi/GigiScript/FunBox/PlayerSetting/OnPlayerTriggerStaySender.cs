
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OnPlayerTriggerStaySender : UdonSharpBehaviour
{
    [SerializeField] PlayerMovementSetter playerMovementSetter;

    BoxCollider thisCollider;
    float colliderHeight;
    private void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
        colliderHeight = transform.position.y + thisCollider.center.y + thisCollider.size.y / 2;
    }
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if(player.isLocal)
        {
            playerMovementSetter.OnPlayerStay(player);
        }
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            playerMovementSetter.OnPlayerEnter(player);
            playerMovementSetter.SetProgramVariable("borderHeight", colliderHeight);
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
