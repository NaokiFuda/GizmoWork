
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayOneshotAndParticle : UdonSharpBehaviour
{
    [SerializeField] AudioClip sound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem targetParticle;

    public void Activate()
    {
        if(audioSource == null) audioSource = GetComponent<AudioSource>();
        if(targetParticle == null) targetParticle = GetComponent<ParticleSystem>();
        audioSource.PlayOneShot(sound);
        targetParticle.Play();
    } 
}
