
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ActivateRenderer : UdonSharpBehaviour
{
    Renderer _renderer;
    [SerializeField] bool isLocal;
    ActivateRenderer _activateRenderer;
    private void Start()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        _activateRenderer = GetComponent<ActivateRenderer>();
    }
    public void Activate()
    {
        _renderer.enabled = !_renderer.enabled;
    }
    public override void Interact()
    {
        if (isLocal) Activate();
        else
        {
            _activateRenderer.SendCustomNetworkEvent(NetworkEventTarget.All , "Activate");
        }
    }

}
