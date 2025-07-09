
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerStateManager : UdonSharpBehaviour
{
    [UdonSynced]
    private int _playerState;
}
