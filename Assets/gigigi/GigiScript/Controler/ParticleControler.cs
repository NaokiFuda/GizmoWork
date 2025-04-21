
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ParticleControler : UdonSharpBehaviour
{
    [SerializeField] GameObject Target;
    private ParticleSystem Particle;
    public bool IsLocal = false;
    public bool IsDeActivate = false;

    void Start()
    {
        Particle = Target.GetComponent<ParticleSystem>();
    }

    public override void Interact()
    {
        if (IsDeActivate)
        {
            if (IsLocal == false)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "DeActivate");
            }
            if (IsLocal == true)
            {
                SendCustomEvent("DeActivate");
            }
        }

        else
        {
            if (IsLocal == false)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
            }
            if (IsLocal == true)
            {
                SendCustomEvent("Activate");
            }
        }
        
    }
    public void Activate()
    {
        Particle.Play(true);
    }
                 
      public void DeActivate()
    {
        Particle.Pause(true);
    }

}
