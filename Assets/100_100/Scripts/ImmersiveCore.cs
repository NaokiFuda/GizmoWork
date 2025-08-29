
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ImmersiveCore : UdonSharpBehaviour
{
    [SerializeField] protected string gameTitle;
    [SerializeField] protected string[] charactersName;
    public string[] GetCharactersName {  get => charactersName;  }
    
    [SerializeField] DisplayText textSender;
    public DisplayText GetDisplayText { get => textSender; }
    [SerializeField] GameObject plSelectPrefab;
    [SerializeField] Transform SyncObjFolder;
    [SerializeField] Transform gameStartSwitch;
    [SerializeField] Transform gmName;
    TextMeshPro _textPro;
    protected bool _gameMaster;
    protected bool _spectator;
    

    VRCPlayerApi[] selectPlayerList = new VRCPlayerApi[20];

    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        GameObject[] _plSelectObj = new GameObject[charactersName.Length];

        int gmNum = 0;
        SetPlayerButton theButton = null;
        for (int i = 0; i < charactersName.Length; i++)
        {
            _plSelectObj[i] = Instantiate(plSelectPrefab, transform.position + Vector3.up * 1.4f + Vector3.left * (charactersName.Length / 2 - i), transform.rotation);
            theButton = _plSelectObj[i].GetComponent<SetPlayerButton>();

            theButton.immersiveCore = this;
            theButton.characterID = i;
            theButton.headFollower = textSender.transform.GetChild(0);

            var t = _plSelectObj[i].transform.GetChild(0);
            _textPro = t.GetComponent<TextMeshPro>();
            if (_textPro != null) _textPro.text = charactersName[i];
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
        theButton.immersiveCore = this;
        theButton.headFollower = textSender.transform.GetChild(0);
        theButton.syncObj = gmName;
    }

    public void AssignPlayer(VRCPlayerApi player,int id)
    {
        if (id < 0)
        {
            selectPlayerList[charactersName.Length] = player; //GM
            SendCustomNetworkEvent(NetworkEventTarget.All, "StartGame");
        }
        else {
            selectPlayerList[id] = player;
        }

    }

    public void StartGame()
    {

    }
}
