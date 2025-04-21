
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SEPlayer : UdonSharpBehaviour
{
    public AudioClip sound1;
    public AudioClip sound2;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        audioSource.PlayOneShot(sound1);
    }
    public void DeActivate()
    {
        audioSource.PlayOneShot(sound2);
    }
}
