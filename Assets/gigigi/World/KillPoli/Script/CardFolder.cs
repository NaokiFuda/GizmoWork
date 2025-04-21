
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CardFolder : UdonSharpBehaviour
{
    [SerializeField] int KeyLayer = 27;
    private GameObject TheCard;
    private Vector3 RockPos;
    private Vector3 FolderPos;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == KeyLayer)
        {
            TheCard = other.gameObject;
            RockPos = TheCard.transform.position;
            FolderPos = transform.position;
            TheCard.transform.parent = this.transform;
            if (!Networking.IsOwner(Networking.LocalPlayer, TheCard))
            {
                Networking.SetOwner(Networking.LocalPlayer, TheCard);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == KeyLayer)
        {
            TheCard = other.gameObject;
            TheCard.transform.parent = this.transform.parent.parent;
        }
    }

}
