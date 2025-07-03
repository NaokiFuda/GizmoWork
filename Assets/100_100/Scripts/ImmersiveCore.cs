
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ImmersiveCore : UdonSharpBehaviour
{
    [SerializeField] string gameTitle;
    [SerializeField] string[] players;
    GameObject[] _plSelectObj;
    [SerializeField] DisplayText textSender;
    [SerializeField] GameObject plSelectPrefab;
    TextMeshProUGUI _textPro;
    bool _gameMaster;
    bool _spectator;
    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        _plSelectObj = new GameObject[players.Length];
        for (int i =0; i< players.Length ; i++)
        {
            _plSelectObj[i] = Instantiate(plSelectPrefab, transform.position + Vector3.up * 1.4f + Vector3.left* (players.Length / 2) * i, transform.rotation);
            _textPro = _plSelectObj[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _textPro.text = players[i];
        }
    }
}
