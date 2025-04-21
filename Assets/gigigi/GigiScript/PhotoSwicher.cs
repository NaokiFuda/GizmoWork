
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PhotoSwicher : UdonSharpBehaviour
{
    [SerializeField] GameObject[] Signals;
    private int i = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AlbumNum))] private int albumNum = 0;
    private int signalNum = 0;
    [UdonSynced(UdonSyncMode.None)] private bool first = true;

    void Start()
    {
        signalNum = Signals.Length;
    }
    //↓この｛｝内のメソッドにコード書くとプレイヤーがjoinしたときにギミックが発動する遊びができる
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
       
    }
    //↓お察しの通り、ここはプレイヤーがいなくなった時にギミックが発動する遊びができる
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        
    }

    //↓この｛｝内にあるメソッドがボタンを押したときの動作
    public override void Interact()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            AlbumNumCheker();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, "AlbumNumCheker");
        }
    }

    public void AlbumNumCheker()
    {
        albumNum++;
        if (albumNum > signalNum)
        {
            albumNum = 1;
        }
        if (first)
        {
            for (i = 0; i < signalNum; i++)
            {
                if (Signals[i].gameObject.activeSelf)
                {
                    albumNum = i + 2;
                    if (albumNum > signalNum)
                    {
                        albumNum = 1;
                    }
                }
            }
            first = false;
        }
        RequestSerialization();
        Activate();
    }

    public int AlbumNum
    {
        get => albumNum;
        set
        {
            albumNum = value;
            Activate();
        }
    }

    public void Activate()
    {
        for (i = 0; i < signalNum; i++)
        {
            if (i != albumNum-1)
            {
                if (Signals[i].gameObject.activeSelf)
                {
                    Signals[i].gameObject.SetActive(false);
                }
            }
            else
            {
                Signals[i].gameObject.SetActive(true);
            }
        }
    }

}
