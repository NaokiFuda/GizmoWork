
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerStateManager : UdonSharpBehaviour
{
    [UdonSynced]
    private int _playerState;

    public static PlayerStateManager GetInstance(VRCPlayerApi player)
    {
        GameObject[] playerObjectList = Networking.GetPlayerObjects(player);
        if (!Utilities.IsValid(playerObjectList))
        {
            return null;
        }

        foreach (GameObject playerObject in playerObjectList)
        {
            if (!Utilities.IsValid(playerObject))
            {
                continue;
            }

            PlayerStateManager foundScript = playerObject.GetComponentInChildren<PlayerStateManager>();
            if (!Utilities.IsValid(foundScript))
            {
                continue;
            }

            return foundScript;
        }

        return null;
    }

    public void AddCount(int count)
    {
        _playerState += count;

        RequestSerialization();
    }
}
