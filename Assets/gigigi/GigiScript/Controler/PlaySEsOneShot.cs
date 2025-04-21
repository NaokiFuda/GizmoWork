
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
    public int playIndex;
    AudioSource audioSource;
    public bool trigerJump;
    public Vector2 inputMovement;
    bool playerStayInArea;
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
    private void OnTriggerExit(Collider other)
    {
        if (hitReact && other.gameObject.layer == 4)
        {
            playerStayInArea = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (stayReact && !playerStayInArea && other.gameObject.layer == 4)
        {
            playerStayInArea = true;
        }
    }
    private void FixedUpdate()
    {
        if (playerStayInArea)
        {
            if (inputMovement != Vector2.zero)
            {
                playIndex = staySEIndex;
                if ( !audioSource.isPlaying) PlayAction(playIndex, inputMovement.magnitude);

            }
            if (trigerJump) 
            {
                playIndex = hitSEIndex;
                if(!audioSource.isPlaying) Activate();
            }
        }
        
    }
    void PlayAction(int index)
    {
        audioSource.PlayOneShot(audioClipList[index]);
    }
    void PlayAction(int index,float roudness)
    {
        audioSource.PlayOneShot(audioClipList[index], roudness);
    }

}
