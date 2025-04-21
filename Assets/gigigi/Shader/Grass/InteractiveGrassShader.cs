
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InteractiveGrassShader : UdonSharpBehaviour
{
    [SerializeField] private Renderer glassRenderer;
    string _interactorPosPropaty = "_InteractorPosition";

    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            glassRenderer.material.SetVector(_interactorPosPropaty, player.GetPosition());
        }
    }
}
