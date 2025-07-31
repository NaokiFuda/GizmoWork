
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
        [SerializeField] GiGiWorldSettings worldSettings;
        [SerializeField] GameObject gameUI;
        [SerializeField] Transform playerHand;
        [SerializeField] float peeklingSize = 0.01f;
        float _defSize = 1;
        Collider[] handsCollider;
        Transform[] handsTransform;

        private void Start()
        {
            handsTransform = new Transform[playerHand.childCount];
            handsCollider = new Collider[playerHand.childCount];
            for (int i = 0; i < playerHand.childCount; i++) 
            {
                handsTransform[i] = playerHand.GetChild(i).transform;
                handsCollider[i] = handsTransform[i].GetComponent<Collider>();
            }
        }
        private void Update()
        {
            for (int i = 0; i < playerHand.childCount; i++)
            {
                IsHandInContact(i);
            }
        }
        private void FixedUpdate()
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            for (int i = 0; i < playerHand.childCount; i++)
                 handsTransform[i].position = localPlayer.GetTrackingData((VRCPlayerApi.TrackingDataType)(i + 1)).position; 
        }

        private void IsHandInContact(in int i)
        {
            for (int j = 1; j <= VRCPlayerApi.GetPlayerCount(); j++)
            {
                VRCPlayerApi player = VRCPlayerApi.GetPlayerById(j);
                Vector3 peeklingPos = player.GetPosition() + (player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position - player.GetPosition()) / 2;
                if (handsCollider[i].bounds.Contains(peeklingPos)) ;
            }
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
            if (mePlayer == -1) mePlayer = Random.Range(1, VRCPlayerApi.GetPlayerCount());
            RequestSerialization();
            SetPlayers();

        }
        public void SetPlayers()
        {
            SetUI(false);
            if (Networking.LocalPlayer.playerId == mePlayer)
            {
                SetSize(_defSize);
            }
            else
            {
                SetSize(peeklingSize);
            }
        }
        public void EndGame()
        {
            SetUI(true);
            SetSize(_defSize);
        }
        public void SetSize(float size)
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            localPlayer.SetAvatarEyeHeightByMultiplier(size);
            if (size != 1) size = 0.5f;
            SetSpeed(localPlayer, size);
        }
        public void SetSpeed(in VRCPlayerApi player,in float amount)
        {
            float factor = (amount == 0) ? 0f : 1f / amount;
            player.SetJumpImpulse(worldSettings.jump * factor);
            player.SetGravityStrength(worldSettings.gravity * factor);

            player.SetWalkSpeed(worldSettings.walkSpeed * amount);
            player.SetRunSpeed(worldSettings.runSpeed * amount);
            
        }
        public void SetUI(bool isIngame)
        {
            gameUI.SetActive(isIngame);
        }

    }
}
