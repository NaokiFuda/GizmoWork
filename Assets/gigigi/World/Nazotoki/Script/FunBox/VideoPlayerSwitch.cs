
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VideoPlayerSwitch : UdonSharpBehaviour
{
    public GameObject[] videoPlayer;
    public GameObject QuestSpeaker;
    void Start()
    {
        if (Networking.LocalPlayer.displayName == "Gizmo-pants")
        {
            GetComponent<Collider>().enabled = true;
            GetComponent<Renderer>().enabled = true;

        }

    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Activator");

    }
    public void Activator()
    {
#if !UNITY_ANDROID
        QuestSpeaker.SetActive(false);
#endif
#if UNITY_ANDROID
        videoPlayer[0].SetActive(!videoPlayer[0].activeSelf);
            videoPlayer[1].SetActive(!videoPlayer[1].activeSelf);  
#endif
    }
}
