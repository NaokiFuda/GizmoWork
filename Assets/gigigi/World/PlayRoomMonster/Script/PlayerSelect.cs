
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PlayRoomMonster
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerSelect : UdonSharpBehaviour
    {
        [UdonSynced, FieldChangeCallback(nameof(MePlayer))] int mePlayer = -1;
        [SerializeField]GiGiWorldSettings worldSettings;
        float _defSize = 1;

        private void Start()
        {

        }
        public int MePlayer
        {
            set { mePlayer = value;  SetPlayers(); } 
        }
        int[] _bufferList;

        public void AddPlayerToMe(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
            mePlayer = player.playerId;
        }
        public void StartGame()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "RequestOwner");
            
        }
        public void RequestOwner()
        {
            if (mePlayer == -1) mePlayer = Random.Range(0, VRCPlayerApi.GetPlayerCount()- 1);
            RequestSerialization();
            if (Networking.LocalPlayer.playerId == mePlayer)
            {
                SetPlayers();
            }
            else
            {
                SetPlayers();
            }
            
        }
        public void SetPlayers()
        {
            //SetAvatarEyeHeightByMultiplier
        }

    }
}
