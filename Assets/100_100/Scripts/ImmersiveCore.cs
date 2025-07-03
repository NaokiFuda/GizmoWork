
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ImmersiveCore : UdonSharpBehaviour
{
    [SerializeField] string gameTitle;
    [SerializeField] string[] players;
    [SerializeField] 
    bool gameMaster;
    bool spectator;
    public void Initialize()
    {

    }
}
