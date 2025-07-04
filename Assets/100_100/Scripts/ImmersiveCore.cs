
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
    TextMeshProUGUI _textPro;
    protected bool _gameMaster;
    protected bool _spectator;
    [SerializeField] GameObject syncManager;
    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        _plSelectObj = new GameObject[playersName.Length];
        
        for (int i =0; i< playersName.Length ; i++)
        {
            _plSelectObj[i] = Instantiate(plSelectPrefab, transform.position + Vector3.up * 1.4f + Vector3.left* (playersName.Length / 2) * i, transform.rotation);
            _plSelectObj[i].GetComponent<SetPlayerButton>().immersiveCore = this;
           
            _textPro = _plSelectObj[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            //_textPro.text = players[i];
        }
        syncManager.SetActive(false);
        syncManager.SetActive(true);
    }
}
