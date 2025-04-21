
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GiGiCardMenu : UdonSharpBehaviour
{
    GiGiCardSetter cardSetter;
    [SerializeField] bool isReset;
    [SerializeField] bool setHandCard;
    [SerializeField] bool _isDrawSingleCardButton;
    [SerializeField] private bool _isDrawSigleForPlayerButton;
    [SerializeField] GameObject _handFolder;
    [FieldChangeCallback(nameof(DeckSwitch))]
    [UdonSynced(UdonSyncMode.None)] public int _deckSwitch = 0;
    [Header("手札の山を0番に登録すること")]
    [SerializeField] GiGiCardSetter[] cardDeck;
    GiGiCardMenu[] singleDrawButton;

    private void Start()
    {
        if(transform.parent.parent.GetComponent<GiGiCardSetter>())
        {
            cardSetter = transform.parent.parent.GetComponent<GiGiCardSetter>();
        }
        if (!isReset && !_isDrawSigleForPlayerButton && !_isDrawSingleCardButton)
        {
            singleDrawButton = new GiGiCardMenu[_handFolder.transform.childCount];
            for (int i = 0; i < _handFolder.transform.childCount; i++)
            {
                if (_handFolder.transform.GetChild(i).GetChild(6).GetComponent<GiGiCardMenu>())
                    singleDrawButton[i] = _handFolder.transform.GetChild(i).GetChild(6).GetComponent<GiGiCardMenu>();
            }
        }
    }
    public int DeckSwitch
    {
        get => _deckSwitch;
        set
        {
            _deckSwitch = value;
            RequestSerialization();
        }
    }
    public override void Interact()
    {
        if (_isDrawSingleCardButton)
        {
            cardSetter.SendCustomEvent("DrawSingleCard");
            return;
        }
        if (isReset) { cardSetter.SendCustomNetworkEvent(NetworkEventTarget.Owner, "ResetAll"); }
        else if (setHandCard)
        {
            int switchNum = SetMeWhich();
            cardSetter.SendCustomNetworkEvent(NetworkEventTarget.All, "SetThisHandCard");
            cardDeck[1 - switchNum].SendCustomNetworkEvent(NetworkEventTarget.All, "SetThisTheme");
            SendCustomNetworkEvent(NetworkEventTarget.All, "SwitchDeckWithHandCard");
        }
        else if (_isDrawSigleForPlayerButton)
        {
            cardDeck[_deckSwitch].SendCustomEvent( "PassSingleCardFromOwner");
        }
        else
        {
            cardSetter.SendCustomNetworkEvent(NetworkEventTarget.All, "SetThisTheme");
            cardDeck[1 - SetMeWhich()].SendCustomNetworkEvent(NetworkEventTarget.All, "SetThisHandCard");
            SendCustomNetworkEvent(NetworkEventTarget.All, "SwitchDeckWithHandCard");
        }
        if (!_isDrawSigleForPlayerButton)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
    public int SetMeWhich() 
    {
        int myIndex = 0;
        for (int i = 0; i < cardDeck.Length; i++)
        {
            if (cardSetter == cardDeck[i])
            {
                myIndex = i; break;
            }
        }
        return myIndex;
    }
    public void SwitchDeckWithHandCard()
    {
        int switchNum = SetMeWhich();
        if (setHandCard)
        {
            for (int i = 0; i < singleDrawButton.Length; i++)
            {
                if (singleDrawButton[i].gameObject.activeSelf)
                {
                    singleDrawButton[i].SetProgramVariable("_deckSwitch", switchNum);
                }
            }
        }
        else
        {
            for (int i = 0; i < singleDrawButton.Length; i++)
            {
                //セットされてるデッキが二つだけでないと動作が狂う手抜きコード。1 - switchNumは自分でないデッキを選ぶの意。
                if (singleDrawButton[i].gameObject.activeSelf)
                    singleDrawButton[i].SetProgramVariable("_deckSwitch", 1 - switchNum);
            }
        }
    }
}