
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GiGiCardSetter : UdonSharpBehaviour
{
    [Header("関連付け必須")]
    [Tooltip("カードが入ったフォルダ")]
    [SerializeField] private GameObject _cardFolder;
    [Tooltip("手札カードを引いた時に出現させる位置")]
    [SerializeField] private GameObject _handFolder;
    [Tooltip("Dealerの切り替えを各ボタンに送るための")]
    [SerializeField] private HandCardPutter[] _dealerObjs;
    [SerializeField, Tooltip("お題カードの転移場所")] private GameObject _display;
    [Tooltip("同時に何枚引くか")]
    [SerializeField] private int _drawOnceNumber = 1;
    private int _drawOnceCount;
    [Tooltip("GiGiCardPlayerSetterを設定したゲーム参加ボタン")]
    [SerializeField] private GameObject _joinGameObject;

    [Tooltip("複数枚引いたカードをどれくらいズラして配置するか")]
    [SerializeField] private float _CardWidth = 0.15f;
    [Tooltip("山札に何枚入っているか")]
    [SerializeField]
    [UdonSynced(UdonSyncMode.None)] private bool[] _drawedCardCheck = new bool[100];

    [UdonSynced(UdonSyncMode.None)] private int _cardsCount = 0;
    private int _lastCardsCount = 0;

    [UdonSynced(UdonSyncMode.None)] private int _drawedCardIndex;
    [Tooltip("カードをすべて山札に戻すのに必要な長押しの時間")]
    [SerializeField] float _resetHoldTime = 2.0f;
    [Tooltip("これが手札ならチェックする")]
    [SerializeField] bool _isHandCard;
    [FieldChangeCallback(nameof(JoinedPlayerIndex))]
    [UdonSynced(UdonSyncMode.None)] private int _joinedPlayerIndex = -1;
    private int[] _joinedPlayerIndexList = new int[10];
    private int _joinedPlayerCount;
    [UdonSynced(UdonSyncMode.None)] private int _drawedPlayerID;
    

    private bool _isInteract;
    private float _interactedHoldTimer;
    private bool _menuOpened;

    Vector3 cardScale;
    float _defcardScale;
    private GameObject[] _cardsList;
    private bool _drawSingle;
    private bool _isDrawing;

    [SerializeField] bool _setGameWithOneShot = true;
    
    void Start()
    {
        _cardsList = new GameObject[_cardFolder.transform.childCount];
        for(int i = 0; i < _cardsList.Length; i++)
        {
            _cardsList[i] = _cardFolder.transform.GetChild(i).gameObject;
        }
        cardScale = transform.localScale;
        _defcardScale = cardScale.y;
    }
    int passingCardsCount = 0;
    int drawForAllCount = 0;
    int drawForAllIndex = 0;
    int finishedCount = 0;
    int passingCountindex = 0;
    void Update()
    {
        if (_isInteract)
        {
            _interactedHoldTimer += Time.deltaTime;
        }
        if (_isPassIngCard)
        {
            if (!_drawSingle)
            {
                while (passingCardsCount < _joinedPlayerIndexList.Length && !_drawSingle)
                {
                    if (_joinedPlayerIndexList[passingCardsCount] != Networking.LocalPlayer.playerId && _joinedPlayerIndexList[passingCardsCount] != 0)
                    {
                        _drawSingle = true;
                        passingCountindex = passingCardsCount;
                        finishedCount++;
                        OwnerDraw(_joinedPlayerIndexList[passingCountindex]);
                    }
                    passingCardsCount++;
                }
            }
        }
        
        if (_isDrawForAll)
        {
            if (!_isDrawing)
            {
                while (drawForAllCount < _joinedPlayerIndexList.Length && !_isDrawing)
                {
                    if (_joinedPlayerIndexList[drawForAllCount] != 0)
                    {
                        _isDrawing = true;
                        drawForAllIndex = drawForAllCount;
                        finishedCount++;
                        OwnerDraw(_joinedPlayerIndexList[drawForAllIndex]);
                    }
                    drawForAllCount++;
                }
            }
        }
        
    }

    bool _requestInteract;
    private void FixedUpdate()
    {
        if (_requestInteract)
        {
            int j = 0;
            foreach(int i in _joinedPlayerIndexList)
            {
                if (Networking.IsOwner(VRCPlayerApi.GetPlayerById(i),gameObject)) j++;
                if (j >= _joinedPlayerCount) break;
                if (i == _joinedPlayerIndexList[_joinedPlayerIndexList.Length-1])
                {
                    j = 1;
                    _requestInteract= false;
                    StartDraw();
                } 
            }
        }
    }
    public override void Interact()
    {
        if (transform.GetChild(1).gameObject.activeSelf)
        {
            if (_menuOpened)
            {
                _menuOpened = false;
                transform.GetChild(1).gameObject.SetActive(false);
                return;
            }
        }
        else { _menuOpened = false; }
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            _requestInteract = true;
            //_isInteract = true;
        }
        else _isInteract = true;
    }
    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (_isOwnerLeft) _isOwnerLeft = false;
    }
    
    int _joinPlayerCount = 0;
    int _maxPlayerId=0;
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        _joinPlayerCount++;
        if (_maxPlayerId <= _joinPlayerCount - _leftPlayerCount)  _maxPlayerId = _joinPlayerCount; 
        if (player.playerId == Networking.LocalPlayer.playerId)
        {
            LatePlayerSetter();
        }
    }
    bool _isOwnerLeft;
    int _leftPlayerCount = 0;
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        _leftPlayerCount++;
        if (Networking.GetOwner(gameObject) == player) _isOwnerLeft = true;
    }
    public int JoinedPlayerIndex
    {
        get => _joinedPlayerIndex;
        set
        {
            _joinedPlayerIndex = value;
            RequestSerialization();
            if (_joinedPlayerIndex < 0)
            {
                _joinedPlayerIndexList[-_joinedPlayerIndex] = 0;
                _joinedPlayerCount--;
            }
            else
            {
                _joinedPlayerIndexList[_joinedPlayerIndex] = Networking.GetOwner(_joinGameObject).playerId;
                _joinedPlayerCount++;
                return;
            }
        }
    }
    [SerializeField] bool _doAutoReset;
    public override void OnPostSerialization(SerializationResult result)
    {
        if(result.success == false)
        {
            RequestSerialization();
            return;
        }
        if (_drawSingle && !_isPassIngCard)
        {
             _drawSingle = false;
        }
        else if (_drawOnceCount > 0 && _isDrawForAll)
        {
            OwnerDraw(_joinedPlayerIndexList[drawForAllIndex]);
        }
        else if (_drawOnceCount > 0 && _drawOnceNumber > _drawOnceCount)
        {
            OwnerDraw(Networking.LocalPlayer.playerId);
        }
        else if (_lastCardsCount >= _cardsList.Length && _doAutoReset || _cardsCount == 0 && _lastCardsCount !=0)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "SetAllDefault");
        }
    }
    public override void OnDeserialization() 
    {
        for (int i=0; i< _joinedPlayerIndexList.Length; i++)
        {
            if (_joinedPlayerIndexList[i] == Networking.LocalPlayer.playerId  )
            {
               break;
            }
            if(i >= _joinedPlayerIndexList.Length - 1)
            {
                return;
            }
        }
        if(_drawOnceCount == _drawOnceNumber + 1)
        {
            DrawCard(_drawOnceCount);
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "DrawedCheck");
        }
        else if (_cardsCount != _lastCardsCount && _cardsCount > 0)
        {
            _drawOnceCount++;
            int bufferCount = _drawOnceCount;
            DrawCard(_drawOnceCount);
            if (bufferCount >= _drawOnceNumber) SendCustomNetworkEvent(NetworkEventTarget.Owner, "DrawedCheck");
        }
    }
    int drawCheckCount;
    public void DrawedCheck()
    {
        drawCheckCount++;
        if (drawCheckCount >= _joinedPlayerCount - 1)
        {
            drawCheckCount = 0;
            _isDrawing = false;
            if(_isPassIngCard)_drawSingle=false;
            if (finishedCount >= _joinedPlayerCount)
            {
                finishedCount = 0;
                drawForAllCount = 0;
                _isDrawForAll = false;
            }
            if (_isPassIngCard && finishedCount >= _joinedPlayerCount-1 )
            {
                finishedCount = 0;
                 passingCardsCount = 0;
                _isPassIngCard = false; 
            }
        }
    }
    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        if (_isInteract && Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            if (_interactedHoldTimer > _resetHoldTime)
            {
                _interactedHoldTimer = 0;
                _isInteract = false;
                _menuOpened = true;
                transform.GetChild(1).gameObject.SetActive(true);

            }
            else if (_interactedHoldTimer > 0.03f)
            {
                _interactedHoldTimer = 0;
                _isInteract = false;

                StartDraw();
            }
        }
    }
    public void StartDraw()
    {
        if (_isDrawing) return;

        if (!_isHandCard)
        {
            OwnerDraw(Networking.LocalPlayer.playerId);
            if (_joinedPlayerCount > 1) _isDrawing = true;

            for (int i = 0; i < _dealerObjs[0].transform.parent.parent.childCount; i++)
            {
                if (_dealerObjs[i].gameObject.activeInHierarchy && Networking.GetOwner(_dealerObjs[i].transform.parent.gameObject) == Networking.LocalPlayer)
                {
                    if (_dealerObjs[i].transform.parent.GetChild(7).gameObject.activeInHierarchy) return;
                    _dealerObjs[i].SendCustomNetworkEvent(NetworkEventTarget.All, "SetDealer");
                    SendCustomNetworkEvent(NetworkEventTarget.All, "SetDealer");
                    return;
                }
            }
        }

        else if (_joinedPlayerCount == 1)
        {
            for (int i = 0; i < _drawOnceNumber; i++)
            {
                OwnerDraw(Networking.LocalPlayer.playerId);
            }
        }

        else if (_setGameWithOneShot && _isHandCard) _isDrawForAll = true;
        else
        {
            OwnerDraw(Networking.LocalPlayer.playerId);
            _isDrawing = true;
        }
    }
    public void OwnerDraw(int drawedPlayerID)
    {
        _drawedCardIndex = Random.Range(0, _cardsList.Length);
        if (_cardsList.Length <= _cardsCount)
        {
            return;
        }
        if (_drawedCardCheck[_drawedCardIndex] != false)
        {
            OwnerDraw(drawedPlayerID);
            return;
        }
         _drawedPlayerID = drawedPlayerID;
        _drawedCardCheck[_drawedCardIndex] = true;
        _cardsCount++;

        if (_drawSingle)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "SetDrawOnceCount");
            _drawOnceCount = _drawOnceNumber + 1;
            return;
        }
        else
        {
            _drawOnceCount++;
        }
        RequestSerialization();
        DrawCard(_drawOnceCount);
    }
    
    public void DrawCard(int cardCount)
    {
        var drawedCard = _cardsList[_drawedCardIndex];
        drawedCard.SetActive(true);

        if (_drawedPlayerID == Networking.LocalPlayer.playerId)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, drawedCard))
            {
                Networking.SetOwner(Networking.LocalPlayer, drawedCard);
            }
            SetDrawCardsPosition(drawedCard, cardCount);
        }
        
        _lastCardsCount = _cardsCount;
        if (_drawOnceNumber <= _drawOnceCount)
        {
            _drawOnceCount = 0;
        }
        cardScale.y = _defcardScale * (_cardsList.Length - _cardsCount) / _cardsList.Length;
        transform.localScale = cardScale;
    }
    public void SetDrawCardsPosition(GameObject cardObject,int cardCount)
    {
        var handingNumber = 0;
        for (int j = 0; j < _joinedPlayerIndexList.Length; j++)
        {
            if (_joinedPlayerIndexList[j] == _drawedPlayerID)
            {
                handingNumber = j;
                break;
            }
        }
        float adjustment = 0;
        if (_drawOnceNumber % 2 != 0)
        {
            adjustment = 0.5f;
        }
        if (!_isHandCard)
        {
            cardObject.transform.SetPositionAndRotation(_display.transform.position, new Quaternion(0.5f, 0, 0, 0.5f));
        }
        else
        {
            if (handingNumber != -1)
            {
                var hand = _handFolder.transform.GetChild(handingNumber);
                cardObject.transform.localPosition = hand.position - cardObject.transform.parent.position + hand.up * 0.2f;
                cardObject.transform.rotation = hand.rotation * new Quaternion(0.5f, 0, 0, 0.5f) * cardObject.transform.rotation;
                Vector3 offset = hand.right * _CardWidth * (cardCount - adjustment - _drawOnceNumber / 2f);
                if (cardCount == _drawOnceNumber + 1)
                {
                    offset += new Vector3(0, Random.Range(0f, 0.1f), Random.Range(0f, 0.03f));
                }
                cardObject.transform.localPosition += offset;
            }
            else
            {
                cardObject.transform.SetPositionAndRotation(_handFolder.transform.position + Vector3.right * _CardWidth * (cardCount - adjustment - _drawOnceNumber / 2f),
                    new Quaternion(-0.5f, 0, 0, 0.5f));
            }
        }
    }
    public void SetDealer()
    {
        for (int i = 0; i < _handFolder.transform.childCount; i++)
        {
            if (_handFolder.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                if (_joinedPlayerIndexList[i] == _drawedPlayerID)
                {
                    _handFolder.transform.GetChild(i).GetChild(5).GetComponent<SpriteRenderer>().enabled= false;
                    _handFolder.transform.GetChild(i).GetChild(5).GetComponent<BoxCollider>().enabled = false;
                    _handFolder.transform.GetChild(i).GetChild(6).gameObject.SetActive(true);
                    _handFolder.transform.GetChild(i).GetChild(7).gameObject.SetActive(true);
                }
                else
                {
                    _handFolder.transform.GetChild(i).GetChild(5).GetComponent<SpriteRenderer>().enabled = true;
                    _handFolder.transform.GetChild(i).GetChild(5).GetComponent<BoxCollider>().enabled = true;
                    _handFolder.transform.GetChild(i).GetChild(6).gameObject.SetActive(false);
                    _handFolder.transform.GetChild(i).GetChild(7).gameObject.SetActive(false);
                }
            }
        }

    }
    public void SetThisHandCard()
    {
        _isHandCard = true;
        _drawOnceNumber = 7;
    }
    public void SetThisTheme()
    {
        _isHandCard = false;
        _drawOnceNumber = 1;
    }
    public void DrawSingleCard()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        _drawSingle = true;
        OwnerDraw(Networking.LocalPlayer.playerId);
    }
    
    int checkDrawSingleCount;
    public void SetDrawOnceCount()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            _drawOnceCount = _drawOnceNumber+ 1;
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "SetDrawOnceCount");
        }
        else
        {
            checkDrawSingleCount++;
            if(checkDrawSingleCount >= VRCPlayerApi.GetPlayerCount())
            {
                RequestSerialization();
                DrawCard(_drawOnceCount);
                checkDrawSingleCount = 0;
            }
        }
    }
    bool _isPassIngCard;
    public void PassSingleCardFromOwner()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            _isPassIngCard = true;
        }
        else
        {
            _isPassIngCard = true;
        }
    }
    bool _isDrawForAll;
    public void DrawForAll()
    {
        _isDrawForAll = true;
    }
    public void ResetAll()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            _cardsCount = 0;
            for (int i = 0; i < _drawedCardCheck.Length; i++)
            {
                _drawedCardCheck[i] = false;
            }
            RequestSerialization();
            if(VRCPlayerApi.GetPlayerCount() == 1)
            {
                SetAllDefault();
            }
        }
    }
    public void SetAllDefault()
    {
        transform.localScale = new Vector3(1,1,1);
        _lastCardsCount = 0;
        _drawOnceCount = 0;
        drawForAllCount = 0;
        drawForAllIndex = 0;
        finishedCount = 0;
        _isDrawForAll =false;
        for (int i = 0; i < _drawedCardCheck.Length; i++)
        {
            _cardsList[i].SetActive(false);
            _cardsList[i].transform.rotation = new Quaternion(1f, 0, 0, 0f);
        }
    }
   
    public void LatePlayerSetter()
    {
        for (int i = 0; i < _cardsList.Length; i++)
        {
            if(_drawedCardCheck[i] == true)
            {
                _cardsList[i].SetActive(true);
            }
        }
    }
}

