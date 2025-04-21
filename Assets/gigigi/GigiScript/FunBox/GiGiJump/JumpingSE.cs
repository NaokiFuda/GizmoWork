
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class JumpingSE : UdonSharpBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    AudioSource audioSource;

    private bool IsJump = false;
    private bool SecoundJump = false;
    private bool First = false;
    private float fallenTime;
    public float startTime0 = 0.3f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if (First)
        {
            First = false;
            return;
        }
        if (!IsJump)
        {
            if (Networking.LocalPlayer.GetVelocity().y >= 0)
            {
                audioSource.PlayOneShot(audioClips[0]);
                IsJump = true;
                First = true;
                return;
            }
            if (Networking.LocalPlayer.GetVelocity().y < 0)
            {
                if (fallenTime < startTime0)
                {
                    audioSource.PlayOneShot(audioClips[0]);
                    IsJump = true;
                    First = true;
                    return;
                }
            }
        }
        if (!SecoundJump)
        {
            audioSource.PlayOneShot(audioClips[1]);
            SecoundJump = true;
        }
    }
    private void Update()
    {

        if (IsJump)
        {
            if (Networking.LocalPlayer.GetVelocity().y == 0)
            {
                IsJump = false;
                SecoundJump = false;
                fallenTime = 0;
            }

            if (Networking.LocalPlayer.GetVelocity().y < 0)
            {
                fallenTime += Time.deltaTime;
            }
        }
    }
}
