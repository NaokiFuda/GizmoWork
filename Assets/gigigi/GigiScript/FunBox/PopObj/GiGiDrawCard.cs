
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GiGiDrawCard : UdonSharpBehaviour
{
    private int NumChildren = 0;
    Transform[] children;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(NumLock))] private int randNum = 100;
    [UdonSynced(UdonSyncMode.None)] private Vector3 DrawPosition;
    [UdonSynced(UdonSyncMode.None)] private int lastRandumNum = 1000;
    [UdonSynced(UdonSyncMode.None)] private int InteractOne;
    [UdonSynced(UdonSyncMode.None)] private bool drawChecker = false;
    [UdonSynced(UdonSyncMode.None)] private int PlayersMassKey = 0;

    [SerializeField] private float CardWidth = 0.2f;
    [SerializeField] private float DeckNum = 0;
    [SerializeField] private GameObject UserName;
    [SerializeField] private GameObject UserFolder;
    [SerializeField] private GameObject CopyCardFolder;
    [SerializeField] private bool IgnoreJoniner = false;

    private int PLCounter = 0;
    private int drawnPlayerCount = 0;
    private float drawedCardsNum = 0;
    private int playerLeft = 0;
    private GameObject drawedCard;
    private int lastlastRandumNum = 0;

    private int i = 0;

    private void Start() {
        children = new Transform[this.transform.childCount];
        NumChildren = this.transform.childCount;

        for (i = 0; i < children.Length; ++i){
            children[i] = this.transform.GetChild(i);
        }
    }
    public override void OnPlayerJoined(VRCPlayerApi player){
        PLCounter++;
        if (!IgnoreJoniner){
            
            Instantiate(UserName, UserFolder.transform.position + new Vector3(2 * (PLCounter - 1), 0, 0), Quaternion.identity, UserFolder.transform) ;
            UserFolder.transform.GetChild(PLCounter-1).gameObject.GetComponent<UnityEngine.UI.Text>().text = VRCPlayerApi.GetPlayerById(PLCounter).displayName;
        }
    }
    public override void OnPlayerLeft(VRCPlayerApi player){
        PLCounter--;
        playerLeft++;

    }
    public override void Interact(){
        if (drawChecker) { return; }
        drawChecker = true;
        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject)){
            OwnerDraw();
            return;
        }
        else{
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player){
        if (playerLeft!=0) {
            playerLeft--;
            return;
        }
        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject)){
            OwnerDraw();
        }
    }
    public void OwnerDraw()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject)){
            PlayersMassKey = PLCounter;
            lastlastRandumNum = lastRandumNum;
            DrawPosition = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
            InteractOne = Networking.LocalPlayer.playerId;
            randNum = Random.Range(0, NumChildren);
            if (lastRandumNum == randNum){
                randNum = 100;
                RequestSerialization();
                FixNum();
                return;
            }
            RequestSerialization();
            SendCustomEvent("DrawCard");
        }
    }

    public int NumLock
    {
        get => randNum;
        set{
            randNum = value;
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) {
                if (randNum == 100){
                    if (PlayersMassKey != 0){
                        SendCustomNetworkEvent(NetworkEventTarget.Owner, "FixNum");
                        return;
                    }
                    SendCustomNetworkEvent(NetworkEventTarget.Owner, "FixNum");
                    return; 
                }
                SendCustomEvent("DrawCard");
            }
        }
    }
    public void FixNum(){
        if(PlayersMassKey != 0){
            PlayersMassKey--;
            if (PlayersMassKey == 0) { FixNum(); }
            return;
        }
        randNum = lastRandumNum;
        RequestSerialization();
        DrawCard();
    }
    public void DrawCard()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject)){
            drawedCard = Instantiate(this.transform.GetChild(randNum).gameObject, DrawPosition, Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * Quaternion.Euler(0f, -90f, -90f), CopyCardFolder.transform); 
            lastRandumNum = randNum;
            RequestSerialization();
            DrawCeck();
            return;
        }
        else {
            DrawPosition = UserFolder.transform.GetChild(InteractOne - 1).gameObject.transform.position;

            drawedCard = Instantiate(this.transform.GetChild(randNum).gameObject, DrawPosition, Quaternion.Euler(0f, -90f, -90f), CopyCardFolder.transform);
            if (drawedCardsNum <= 9){
                drawedCard.transform.Translate(DeckNum * 0.6f - 1.0f, 0f, 1.8f - CardWidth * drawedCardsNum);
            }
            if (drawedCardsNum > 9){
                drawedCard.transform.Translate(DeckNum * 0.6f - 1.0f + 0.3f, 0f, 1.8f - CardWidth * (drawedCardsNum -10.0f));
            }
            drawedCardsNum++;
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "DrawCeck");
        }
    }

    public void DrawCeck(){
         drawnPlayerCount++;
        UserFolder.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = drawnPlayerCount+"人同期";
        if (PLCounter == drawnPlayerCount) {
            UserFolder.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = VRCPlayerApi.GetPlayerById(1).displayName;
            drawChecker = false;
            drawnPlayerCount = 0;
            RequestSerialization();
        }
    }
}
