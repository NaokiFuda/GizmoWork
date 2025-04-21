
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class AudioMixing : UdonSharpBehaviour
{
    //[SerializeField] Transform videoPlayer;
    public AudioSource mainSource; // ビデオプレーヤーのオーディオソース
    public AudioSource[] speakerSources; // 各スピーカーのオーディオソース
    public bool movieOn;

    void Update()
    {
        if (!movieOn ) return;

        //if (mainSource.transform.parent != videoPlayer) mainSource.transform.parent = videoPlayer;
        // メインソースの状態を各スピーカーに反映
        for (int i = 0; i < speakerSources.Length; i++)
        {
            //if (videoPlayer != null && speakerSources[i].transform.parent != videoPlayer) speakerSources[i].transform.parent = videoPlayer;
            // ミュート状態を同期
            speakerSources[i].mute = mainSource.mute;
            //if (mainSource.clip != null) speakerSources[i].clip = mainSource.clip;

            // 再生状態を同期
            if (mainSource.isPlaying && !speakerSources[i].isPlaying)
            {
                speakerSources[i].Play();
                speakerSources[i].time = mainSource.time; // タイミングを同期
            }
            else if (!mainSource.isPlaying && speakerSources[i].isPlaying)
            {
                speakerSources[i].Stop();
            }

            // ボリュームを同期（必要に応じて）
            speakerSources[i].volume = mainSource.volume;
        }

    }
    public void SetAudioMixing()
    {
        movieOn = true;
    }
    public void OffAudioMixing()
    {
        movieOn = false;
    }
}
