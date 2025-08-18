
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Wrapper.Modules;

namespace PlayRoomMonster
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GameManager : UdonSharpBehaviour
    {
        [UdonSynced, FieldChangeCallback(nameof(MePlayer))] int mePlayer = -1;
        public VRCPlayerApi GetMePlayer 
        { 
            get  
            {
                if (mePlayer > 0) 
                return VRCPlayerApi.GetPlayerById(mePlayer);
                else return null;
            } 
        }
        [SerializeField] MeActions meAction;
        [SerializeField] PeeklingActions peeklingAction;
        [SerializeField] GiGiWorldSettings worldSettings;
        [SerializeField] GameObject gameUI;
        Transform playerHand;
        [SerializeField] float humanSize = 7.8f;
         float _defSize = 1;
        [SerializeField] bool debugPeekling;
        Collider[] handsCollider;
        Transform[] handsTransform;

        private void Start()
        {
            playerHand = meAction.transform;
            handsTransform = new Transform[playerHand.childCount];
            handsCollider = new Collider[playerHand.childCount];
            for (int i = 0; i < playerHand.childCount; i++) 
            {
                handsTransform[i] = playerHand.GetChild(i).transform;
                handsCollider[i] = handsTransform[i].GetComponent<Collider>();
            }
        }

        private void FixedUpdate()
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            for (int i = 0; i < playerHand.childCount; i++)
                 handsTransform[i].position = localPlayer.GetTrackingData((VRCPlayerApi.TrackingDataType)(i + 1)).position; 
        }


        public int MePlayer
        {
            set 
            { 
                mePlayer = value;
                SetPlayers();

            } 
        }

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
            if (mePlayer == -1 && !debugPeekling) mePlayer = Random.Range(1, VRCPlayerApi.GetPlayerCount());
            RequestSerialization();
            SetPlayers();

        }
        public void SetPlayers()
        {
            SetUI(false);
            bool isMe = Networking.LocalPlayer.playerId == mePlayer;
            peeklingAction.isMe = isMe;
            meAction.isMe = isMe;
            if (isMe)
            {
                SetParameter(humanSize);
            }
            else
            {
                SetParameter(_defSize);
            }
            meAction.isMe = isMe;
        }
        public void EndGame()
        {
            SetUI(true);
            SetParameter(_defSize);
        }
        public void SetParameter(float size)
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            localPlayer.SetAvatarEyeHeightByMultiplier(size);
            if (size != humanSize)
            {

            }
            SetSpeed(localPlayer, size);
        }
        public void SetSpeed(VRCPlayerApi player,float amount)
        {
           // float factor = (amount == 0) ? 0f : 1f * amount;
            player.SetJumpImpulse(worldSettings.jump * amount);
            player.SetGravityStrength(worldSettings.gravity * amount);

            player.SetWalkSpeed(worldSettings.walkSpeed * amount);
            player.SetRunSpeed(worldSettings.runSpeed * amount);
            player.SetStrafeSpeed(worldSettings.runSpeed * amount);


        }
        public void SetUI(bool isIngame)
        {
            gameUI.SetActive(isIngame);
        }

    }
}
