
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PlaySE : UdonSharpBehaviour
{
	[SerializeField] bool IsLocal = false;
	[SerializeField] bool HitCall = false;

	public AudioClip sound1;
    AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
	}
	public override void Interact()
	{
		if (HitCall)
		{ return; }
		if (IsLocal == false)
		{
			SendCustomNetworkEvent(NetworkEventTarget.All, "OneShot");
		}
		else
		{
			audioSource.PlayOneShot(sound1);
		}
		
	}
	void OnCollisionEnter(Collision other)
	{
        if (HitCall)
        {
			if (IsLocal == false)
			{
				SendCustomNetworkEvent(NetworkEventTarget.All, "OneShot");
			}
			else
			{
				audioSource.PlayOneShot(sound1);
			}
		}
	}
	public void OneShot()
    {
        audioSource.PlayOneShot(sound1);
    }
}
