
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class CardSlotMove : UdonSharpBehaviour
{
    [SerializeField] Collider cardSlot;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 27  && Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
        {
            if(cardSlot == null) cardSlot = GetComponent<Collider>();
            RejectCard();
            _card = other.gameObject.GetComponent<Collider>(); ;
            _card.transform.position = cardSlot.bounds.center;
            _card.transform.rotation = cardSlot.transform.rotation;
            _card.transform.parent = cardSlot.transform;
            _card.GetComponent<VRC_Pickup>().pickupable = false;
            DelayedInsert();
        }

        if (other.gameObject.layer == 26 && _rejected && Networking.IsOwner(Networking.LocalPlayer, other.gameObject))
        {
            _rejected = false;
            RejectCard();
        }
    }
    Collider _card;
    void DelayedInsert()
    {
        if (Vector3.SqrMagnitude(transform.position - _card.bounds.center) > 0.1f)
        {
            _card.transform.position += cardSlot.transform.up * -0.02f;
            SendCustomEventDelayedSeconds(nameof(DelayedInsert), 0.2f);
        }
    }
    void RejectCard()
    {
        _card.transform.parent = null;
        _card.GetComponent<VRC_Pickup>().pickupable = true;
        _card.transform.position = cardSlot.bounds.size.y * transform.up + 0.3f * transform.forward;
        _card = null;
    }

    bool _rejected = false;
    bool _trigerStay;
    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        if (_trigerStay && !value) _rejected = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_trigerStay && _card != null) _trigerStay = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!_trigerStay && _card != null) _trigerStay = false;
    }
}
