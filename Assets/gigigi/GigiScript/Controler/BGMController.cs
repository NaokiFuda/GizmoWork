
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BGMController : UdonSharpBehaviour
{
    AudioSource audioSource;
    [SerializeField]AudioClip[] clips;
    private void Update()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    int clipsNum;
    public void Activate()
    {
        if (clips.Length <= 1 && audioSource.isPlaying)
        {
            audioSource.Stop();
            return;
        }
        if (clips.Length > 1 && audioSource.isPlaying)
        {
            clipsNum++;
            if (clipsNum >= clips.Length)
            {
                clipsNum = -1;
                audioSource.clip = clips[0];
                audioSource.Stop();
                return;
            }
            audioSource.clip = clips[clipsNum];
        }
        audioSource.Play();
    }
    public void SwitchPause()
    {
        if(audioSource.isPlaying) audioSource.Pause();
        else audioSource.Play();
    }

    public void DeActivate()
    {
        audioSource.Stop();
    }

    public void SetVolumeDown() 
    {
        audioSource.volume -= 0.1f;
    }
    public void SetVolumeUp()
    {
        audioSource.volume += 0.1f;
    }
}
