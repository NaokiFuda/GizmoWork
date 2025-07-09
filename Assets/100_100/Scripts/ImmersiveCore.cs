
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ImmersiveCore : UdonSharpBehaviour
{
    [SerializeField] protected string gameTitle;
    [SerializeField] protected string[] playersName;
    public string[] GetPlayersName {  get => playersName;  }
    GameObject[] _plSelectObj;
    [SerializeField] DisplayText textSender;
    public DisplayText GetDisplayText { get => textSender; }
    [SerializeField] GameObject plSelectPrefab;
    [SerializeField] Transform SyncObjFolder;
    [SerializeField] Transform gameStartSwitch;
    [SerializeField] Transform gmName;
    TextMeshPro _textPro;
    protected bool _gameMaster;
    protected bool _spectator;

    VRCPlayerApi[] players;

    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        _plSelectObj = new GameObject[playersName.Length];
        players = new VRCPlayerApi[playersName.Length+1];
        int gmNum = 0;
        SetPlayerButton theButton = null;
        for (int i = 0; i < playersName.Length; i++)
        {
            _plSelectObj[i] = Instantiate(plSelectPrefab, transform.position + Vector3.up * 1.4f + Vector3.left * (playersName.Length / 2 - i), transform.rotation);
            theButton = _plSelectObj[i].GetComponent<SetPlayerButton>();

            theButton.immersiveCore = this;
            theButton.playerID = i;
            theButton.headFollower = textSender.transform;

            if (i > 0)
            {
                var t = _plSelectObj[i].transform.GetChild(0);
                _textPro = t.GetComponent<TextMeshPro>();
                if (_textPro != null) _textPro.text = playersName[i];
                for (int j = 0; j < SyncObjFolder.childCount; j++)
                {
                    Transform c = SyncObjFolder.GetChild(j);
                    if (c.childCount == 0)
                    {
                        theButton.syncObj = c;
                        t.parent = c;
                        gmNum = j + 1;
                        break;
                    }
                }
            }
        }
        theButton = gameStartSwitch.GetComponent<SetPlayerButton>();
        theButton.immersiveCore = this;
        theButton.headFollower = textSender.transform;
        theButton.syncObj = gmName;
    }

    public void AssignPlayer(VRCPlayerApi player,int id)
    {
        if (id < 0)
        {
            players[players.Length-1] = player;
            SendCustomNetworkEvent(NetworkEventTarget.All, "StartGame");
        }
        else players[id] = player;

    }
    public void StartGame()
    {

    }
}
