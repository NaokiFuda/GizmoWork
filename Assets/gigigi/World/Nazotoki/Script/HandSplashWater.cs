
using UdonSharp;
using UnityEngine;
using UnityEngine.Android;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;

public class HandSplashWater : UdonSharpBehaviour
{
    AudioSource audioSource;
    [SerializeField] ParticleSystem splashEffect;
    [SerializeField] AudioClip splashSE;
    [SerializeField] private float _timeInterval = 0.1f;
    float timer;
    float handVelocity;
    Vector3 lastHandPos;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4 )
            timer = 0;
            
    }
    private void OnTriggerExit(Collider other)
    {
        if ( other.gameObject.layer == 4 && timer >0.01f)
        {
            audioSource.PlayOneShot(splashSE);
            audioSource.volume = Mathf.Clamp(timer /3 , 0.1f, 1f);
            splashEffect.transform.rotation = Quaternion.LookRotation(transform.position - lastHandPos);

            var bursts = new ParticleSystem.Burst[]
            {
                    new ParticleSystem.Burst(0f, Mathf.Clamp(timer * 100, 3, 100))
            };
            splashEffect.emission.SetBursts(bursts);
            splashEffect.Play();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if ( other.gameObject.layer == 4 )
        {
            lastHandPos = transform.position;
            timer += Time.deltaTime;
            SendCustomEventDelayedSeconds(nameof(DelayedAction), _timeInterval);
        }
    }
    public void DelayedAction()
    {
        handVelocity = Vector3.Distance(transform.position, lastHandPos);
        if (handVelocity < 0.02f)
        {
            timer = 0;
            return;
        }
    }
}
