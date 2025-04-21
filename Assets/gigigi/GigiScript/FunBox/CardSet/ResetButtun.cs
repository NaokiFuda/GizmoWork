
using Cysharp.Threading.Tasks.Triggers;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ResetButtun : UdonSharpBehaviour
{
    [SerializeField] GiGiCardSetter cardSetter;
    [SerializeField] bool isReset;
    [SerializeField] bool isHandCard;
    [SerializeField] private bool _isDrawButton;
    [FieldChangeCallback(nameof(DeckSwitch))]
    [UdonSynced(UdonSyncMode.None)] public int _deckSwitch;
    [SerializeField] GiGiCardSetter[] cardDeck;

    private void Start()
    {
        cardSetter = transform.parent.parent.GetComponent<GiGiCardSetter>();
    }
    public int DeckSwitch
    {
        get => _deckSwitch;
        set
        {
            _deckSwitch = value;
        }
    }
    public override void Interact()
    {
        if (isReset) { cardSetter.SendCustomNetworkEvent(NetworkEventTarget.All, "ResetAll"); }
        else if (isHandCard) { cardSetter.SendCustomNetworkEvent(NetworkEventTarget.All, "SetThisHandCard"); }
        else if (_isDrawButton) 
        {
            cardDeck[_deckSwitch].SendCustomNetworkEvent(NetworkEventTarget.All, "DrawSingleCard"); 
        }
        else { cardSetter.SendCustomNetworkEvent(NetworkEventTarget.All, "SetThisTheme"); }
        
        transform.parent.gameObject.SetActive(false);
    }
}
