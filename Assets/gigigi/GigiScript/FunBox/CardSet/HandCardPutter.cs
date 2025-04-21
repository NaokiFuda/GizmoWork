
using System.Text.RegularExpressions;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class HandCardPutter : UdonSharpBehaviour
{
    private GameObject[] _oldParents = new GameObject[20];
    private int _setNumber = 0;
    [Tooltip("山札に引いたカードの代わりに詰める用空オブジェクト")]
    [SerializeField] private GameObject _BufferFolder;

    [SerializeField] private bool _isTrashButton;
    [SerializeField] private bool _isDealerButton;
    private bool _isCardOn;
    private GameObject _targetObj;
    [SerializeField] private HandCardPutter[] _dealerObj;
    private SpriteRenderer _targetRenderer;
    public int _targetIndex;
    private VRC_Pickup thisPickup;

    //AudioSource _speaker;
    [SerializeField] AudioClip _isMeDealerSE;
    [SerializeField] GiGiCardSetter[] _cardFolder;
    [FieldChangeCallback(nameof(DeckSwitch))]
    [UdonSynced(UdonSyncMode.None)] public int _deckSwitch;

    
    public int DeckSwitch
    {
        get => _deckSwitch;
        set
        {
            _deckSwitch = value;
            RequestSerialization();
        }
    }
    void Start()
    {
        if (_isTrashButton || _isDealerButton)
        {
            //_speaker = GetComponent<AudioSource>();
            _targetRenderer = GetComponent<SpriteRenderer>();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (Networking.GetOwner(gameObject.transform.parent.gameObject) != Networking.LocalPlayer)
        {
            return;
        }
        else if (other.gameObject.layer == 13 && other.gameObject.name[0] == 'C')
        {
            if (_isTrashButton || _isDealerButton)
            {
                _isCardOn = true;
                _targetRenderer.material.color = Color.black;
                thisPickup = other.gameObject.GetComponent<VRC_Pickup>();
            }
            else
            {
                if (_setNumber >= _oldParents.Length) return;
                var cardIndex = int.Parse(Regex.Replace(other.transform.name, @"\D{2,10}|\D{1,}\d", "")) - 1;
                _oldParents[_setNumber] = other.gameObject.transform.parent.gameObject;
                other.gameObject.transform.SetParent(transform, true);
                _BufferFolder.transform.GetChild(0).SetParent(_oldParents[_setNumber].transform, true);
                _oldParents[_setNumber].transform.GetChild(_oldParents[_setNumber].transform.childCount-1).SetSiblingIndex(cardIndex);
                _setNumber++;
                /*
                if (Networking.GetOwner(other.gameObject) != Networking.LocalPlayer)
                {
                    Networking.SetOwner(Networking.LocalPlayer, other.gameObject);
                }
                
                other.gameObject.transform.localRotation = new Quaternion(0.5f, 0, 0, 0.5f) * new Quaternion(0, 0, 1, 0) * new Quaternion(0.5f, 0, 0, 0.5f); 
            */
                }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 13 && other.gameObject.name[0] == 'C')
        {
            if (_isTrashButton || _isDealerButton)
            {
                _isCardOn = true;
                _targetObj = other.gameObject;
            }
        }
    }
    public void SetDealer()
    {
        for(int i = 0; i < transform.parent.parent.childCount; i++)
        {
            if (_dealerObj[i].gameObject.activeInHierarchy)
            {
                _dealerObj[i].SetProgramVariable("_targetIndex", transform.parent.GetSiblingIndex());
            }
        }
    }
    public override void InputGrab(bool value, UdonInputEventArgs args)
    {
        if (_isCardOn && Networking.GetOwner(gameObject.transform.parent.gameObject) == Networking.LocalPlayer)
        {
            _isCardOn = false;
            if (_isTrashButton)
            {
                if (thisPickup) thisPickup.Drop();
                
                _targetObj.transform.localPosition = Vector3.zero;

                if (thisPickup) thisPickup = null;
            }
            else if (_isDealerButton)
            {
                if(thisPickup) thisPickup.Drop();
                if (thisPickup) thisPickup = null;
                _targetObj.transform.position = transform.parent.parent.GetChild(_targetIndex).position;
                _targetObj.transform.localRotation =transform.parent.parent.GetChild(_targetIndex).rotation * new Quaternion(-0.5f, 0, 0, 0.5f);
                _targetObj.transform.localPosition += _targetObj.transform.TransformDirection(Vector3.up * 0.2f + new Vector3(UnityEngine.Random.Range(0f,0.5f) , 0, 0));

                
            }
        }
    } 
    /*
    public override void InputDrop(bool value, UdonInputEventArgs args)
    {
        if (_isCardOn && Networking.GetOwner(gameObject.transform.parent.gameObject) == Networking.LocalPlayer)
        {
            _isCardOn = false;
            if (_isTrashButton)
            {
                _targetObj.transform.localPosition = Vector3.zero;
            }
            else if (_isDealerButton)
            {
                _targetObj.transform.position = transform.parent.parent.GetChild(_targetIndex).position;
                _targetObj.transform.localRotation = transform.parent.parent.GetChild(_targetIndex).rotation * new Quaternion(-0.5f, 0, 0, 0.5f);
                _targetObj.transform.localPosition += _targetObj.transform.TransformDirection(Vector3.up * 0.2f + new Vector3(UnityEngine.Random.Range(0f, 0.5f), 0, 0));
            }
        }
    }
    */
    void OnTriggerExit(Collider other)
    {
        if (Networking.GetOwner(gameObject.transform.parent.gameObject) != Networking.LocalPlayer)
        {
            return;
        }
        else if (other.gameObject.layer == 13 && other.gameObject.name[0] == 'C')
        {
            if (_isTrashButton || _isDealerButton)
            {
                _isCardOn = false;
                _targetRenderer.material.color = Color.white;
                if (thisPickup) thisPickup = null;
            }
            else if(other.transform.parent.gameObject == gameObject)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (other.gameObject == transform.GetChild(i).gameObject)
                    {
                        int cardIndex = int.Parse(Regex.Replace(other.transform.name, @"\D{2,10}|\D{1,}\d", "")) - 1;
                        _oldParents[i].transform.GetChild(cardIndex).SetParent(_BufferFolder.transform, true);
                        other.transform.SetParent(_oldParents[i].transform, true);
                        other.transform.SetSiblingIndex(cardIndex);

                        _oldParents[i] = null;
                        if (i != _oldParents.Length - 1)
                        {
                            for (int j = i ; j < transform.childCount; j++)
                            {
                                _oldParents[j] = _oldParents[j + 1];
                            }
                        }
                        _setNumber--;
                        return;
                    }
                }
            }
            
        }
    }
}
