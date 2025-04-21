

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GiGiCardPlayerSetter : UdonSharpBehaviour
{
    [SerializeField, Tooltip("ハンドのプレハブ")] private GameObject[] _handPrefab;
    [Tooltip("プレイヤーに配る山札を指定")]
    [SerializeField] private GiGiCardSetter[] _cardDeck;
    [UdonSynced(UdonSyncMode.None)] public int _deckSwitch = 0;
    [UdonSynced(UdonSyncMode.None)] private int[] _joinedPlayerIDList = new int[10];
    [UdonSynced(UdonSyncMode.None)] private int _joinedPlayerIndex = -1;
    private bool _isInteracted;
    private bool _isJoined;
    [UdonSynced(UdonSyncMode.None)] private bool _isUnderTask;
    private bool _isOwnerLefted;
    private int _leftedID;
    private bool _lateJoiner;

    public override void Interact()
    {
        if (_isUnderTask) return;
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            _isUnderTask = true;
            RequestSerialization();
        }
        else
        {
            _isInteracted = true;
            _isUnderTask = true;
            RequestSerialization();
        }
    }
    
    void Update()
    {
        if (_isInteracted)
        {
            _isInteracted = false;
            if (_isJoined)
            {
                _isJoined = false;
                for (int i = 0; i < _joinedPlayerIDList.Length; i++)
                {
                    if (_joinedPlayerIDList[i] == Networking.LocalPlayer.playerId)
                    {
                        SetPlayerStatus(i);
                        return;
                    }
                }
            }

            for (int i = 0; i < _joinedPlayerIDList.Length; i++)
            {
                if (_joinedPlayerIDList[i] == 0)
                {
                    SetPlayerStatus(i);

                    _isJoined = true;
                    return;
                }
            }
        }
        if (_isOwnerLefted)
        {
            _isOwnerLefted = false;
            if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                for (int i = 0; i < _joinedPlayerIDList.Length; i++)
                {
                    if (_joinedPlayerIDList[i] == _leftedID)
                    {
                        SetPlayerStatus(i);
                        return;
                    }
                }
            }
        }
    }
    public override void OnPostSerialization(SerializationResult result)
    {
        if (_isUnderTask && _joinedPlayerIndex == -1) _isInteracted = true;
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {

        _leftedID = player.playerId;
        if (Networking.IsOwner(player, gameObject))
        {
            _isOwnerLefted = true;
        }
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            for (int i = 0; i < _joinedPlayerIDList.Length; i++)
            {
                if (_joinedPlayerIDList[i] == _leftedID)
                {
                    SetPlayerStatus(i);
                    return;
                }
            }
        }
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.playerId == Networking.LocalPlayer.playerId) _lateJoiner = true;
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject)) RequestSerialization();
    }
    public override void OnDeserialization()
    {
        if (_lateJoiner)
        {
            _lateJoiner = false;
            for (int i = 0; i < _joinedPlayerIDList.Length; i++)
            {
                if (_joinedPlayerIDList[i] != 0) IncludePlayerForLateJoiner(i);
            }
            return;
        }
        if (!_isUnderTask|| _joinedPlayerIndex == -1)  return;
        if (_joinedPlayerIDList[_joinedPlayerIndex] == Networking.GetOwner(gameObject).playerId) IncludePlayer();
        else if (_joinedPlayerIDList[_joinedPlayerIndex] == 0) ExcludePlayer();
    }
    void SetPlayerStatus(int i)
    {
        if (_joinedPlayerIDList[i] == 0)
        {
            _joinedPlayerIDList[i] = Networking.LocalPlayer.playerId;
            _joinedPlayerIndex = i;
            IncludePlayer();

        }
        else if (_joinedPlayerIDList[i] == _leftedID || _joinedPlayerIDList[i] == Networking.LocalPlayer.playerId)
        {
            _joinedPlayerIDList[i] = 0;
            _joinedPlayerIndex = i;
            ExcludePlayer();

        }
        RequestSerialization();
    }
    public void IncludePlayer()
    {
        for(int i=0;i< _cardDeck.Length; i++)
        {
            _cardDeck[i].SetProgramVariable("_joinedPlayerIndex", _joinedPlayerIndex);
        }
        _handPrefab[_joinedPlayerIndex].SetActive(true);
        _handPrefab[_joinedPlayerIndex].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = VRCPlayerApi.GetPlayerById(_joinedPlayerIDList[_joinedPlayerIndex]).displayName;
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, _handPrefab[_joinedPlayerIndex]);
            var headPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            _handPrefab[_joinedPlayerIndex].transform.localRotation = new Quaternion(0, headPos.rotation.y, 0, headPos.rotation.w);
            _handPrefab[_joinedPlayerIndex].transform.localPosition = headPos.position - _handPrefab[_joinedPlayerIndex].transform.parent.position + _handPrefab[_joinedPlayerIndex].transform.TransformDirection(Vector3.forward * 0.8f);
        }
        SendCustomNetworkEvent(NetworkEventTarget.Owner, "NotifyOwnerCompletion");
    }
    public void ExcludePlayer()
    {
        for (int i = 0; i < _cardDeck.Length; i++)
        {
            _cardDeck[i].SetProgramVariable("_joinedPlayerIndex", -_joinedPlayerIndex);
        }
        _handPrefab[_joinedPlayerIndex].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "player";
        _handPrefab[_joinedPlayerIndex].SetActive(false);
        SendCustomNetworkEvent(NetworkEventTarget.Owner, "NotifyOwnerCompletion");
    }
    int completionCount;
    public void NotifyOwnerCompletion()
    {
        completionCount++;
        if (completionCount >= VRCPlayerApi.GetPlayerCount())
        {
            _isUnderTask = false;
            completionCount = 0;
            _joinedPlayerIndex = -1;
            RequestSerialization();
        }
    }
    void IncludePlayerForLateJoiner(int i)
    {
        _handPrefab[i].SetActive(true);
        _handPrefab[i].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = VRCPlayerApi.GetPlayerById(_joinedPlayerIDList[i]).displayName;
    }
}
