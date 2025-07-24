
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PlayRoomMonster
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerSelect : UdonSharpBehaviour
    {
        [UdonSynced] int mePlayer = -1;
        [UdonSynced] int[] peeksPlayers;
        int[] _bufferList;
        int _peeksPlayerNum = -1;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            _peeksPlayerNum++;
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            _peeksPlayerNum--;
        }
        public void AddPlayerToMe(VRCPlayerApi player)
        {
            mePlayer = player.playerId;
        }
        public void AddPlayerToPeeklings(int playerID)
        {
            foreach (int i in peeksPlayers) if (i == playerID) return;
            _bufferList = peeksPlayers;
            peeksPlayers = new int[_peeksPlayerNum-1];
            for (int i = 0; i < peeksPlayers.Length; i++) peeksPlayers[i] = -1;
            foreach(int id in _bufferList)
            {
                for(int i = 0; i < peeksPlayers.Length;i++)
                {
                    if (id == -1)
                    {
                        peeksPlayers[i] = playerID;
                        break;
                    }
                    else if(peeksPlayers[i] != id)
                    {
                        peeksPlayers[i] = id;
                        break;
                    }
                }
                    
                       
            }
        }
        public void StartGame()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "RequestOwner");
            
        }
        public void RequestOwner()
        {
            if (mePlayer == -1) mePlayer = Random.Range(0, VRCPlayerApi.AllPlayers.Count - 1);
            for (int i = 0; i < _peeksPlayerNum; i++) if (i != mePlayer) AddPlayerToPeeklings(i);
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SetPlayers");
        }
        public void SetPlayers()
        {

        }
    }
}
