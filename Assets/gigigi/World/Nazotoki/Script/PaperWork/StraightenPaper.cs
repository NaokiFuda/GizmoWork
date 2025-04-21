using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class StraightenPaper : UdonSharpBehaviour
{
    [SerializeField]PaperFlatter paperFlatter;
    bool _trigerStaying;
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 26) _trigerStaying = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 26) _trigerStaying = false;
    }
    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        if(_trigerStaying) paperFlatter.DoStraighten();
    }
}
