
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class ContrastChanger : UdonSharpBehaviour
{
    [SerializeField] private GameObject ShaderObj;
    [SerializeField, Range(0.0f, 1.0f)] float _HistMin;
    [SerializeField, Range(0.0f, 1.0f)] float _HistMax;
    [SerializeField, Range(0.0f, 1.0f)] float _AddPercentage;
    [SerializeField, Range(0.800f, 1.200f)] float _NoizeLv=1.0f;
    [SerializeField] private Color _Color = new Color(0.274f, 0.73725f, 0.73725f, 1.0f);
    Material _mat;

    void Start()
    {
        _mat = ShaderObj.GetComponent<Image>().material;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player){
        if (player != Networking.LocalPlayer)
        {
            return;
        }
        _mat.SetFloat("_HistMin", _HistMin);
        _mat.SetFloat("_HistMax", _HistMax);
        _mat.SetFloat("_AddPercentage", _AddPercentage);
        _mat.SetFloat("_NoizeLv", _NoizeLv);
        _mat.SetColor("_Color", _Color);
        this.gameObject.SetActive(false);
    }
}
