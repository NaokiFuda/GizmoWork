
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BGMActivator : UdonSharpBehaviour
{
    AudioSource audioSource;
    public void Activate()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void DeActivate()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.Stop();
    }
}
