
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PlaySEsOneShot : UdonSharpBehaviour
{
    [SerializeField] AudioClip[] audioClipList;
    [SerializeField] bool hitReact;
    [SerializeField] int hitSEIndex;
    [SerializeField] bool stayReact;
    [SerializeField] int staySEIndex;
    public int GetStaySEIndex() { return staySEIndex; }
    public int GetHitSEIndex() {return hitSEIndex; }
    public int playIndex;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        PlayAction(playIndex);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(hitReact && other.gameObject.layer == 4)
        {
            PlayAction(hitSEIndex);
        }
    }
    
    public void PlayAction(int index)
    {
        audioSource.PlayOneShot(audioClipList[index]);
    }
    public void PlayAction(int index,float roudness)
    {
        audioSource.PlayOneShot(audioClipList[index], roudness);
    }

}
