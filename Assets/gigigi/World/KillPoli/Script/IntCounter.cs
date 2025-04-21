
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IntCounter : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(IntCounting))] public int _IntCounting = 0;
    [UdonSynced(UdonSyncMode.None)] public int _PushInt = 0;

    public int IntCounting
    {
        get => _IntCounting;
        set
        {
            _IntCounting = value;
            DisplayCountData();
        }
    }

    public void PushCounter()
    {
        _IntCounting += _PushInt;
        RequestSerialization();
        DisplayCountData();
    }

    public void DisplayCountData()
    {
        Text DisplayDataText = this.gameObject.GetComponent<Text>();
        DisplayDataText.text = _IntCounting.ToString();
    }

}
