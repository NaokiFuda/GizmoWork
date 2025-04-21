
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TriggerParticleEmitter : UdonSharpBehaviour
{
    ParticleSystem particle;
    VRCPlayerApi player;
    [SerializeField] float particleHeight;
    [SerializeField] bool emitFormBottom;
    [SerializeField] bool followPlayer;
    public float borderHeight;
    bool follow;
    public Transform senderTransform;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        player = Networking.LocalPlayer;
    }
    public void Activate()
    {
        if (followPlayer) 
        {
            transform.position = player.GetPosition();
            follow = true;
        }
        else if (particleHeight == 0 || emitFormBottom) transform.position = player.GetPosition();
        else if (senderTransform != null && particleHeight == 0) transform.position = new Vector3(player.GetPosition().x, senderTransform.position.y, player.GetPosition().z); 
        else if (senderTransform != null && particleHeight != 0) transform.position = new Vector3(player.GetPosition().x, senderTransform.position.y + particleHeight, player.GetPosition().z);
        else transform.position = new Vector3(player.GetPosition().x, particleHeight, player.GetPosition().z);
        particle.Play();
    }

    private void FixedUpdate()
    {
        if (follow) transform.position = player.GetPosition();
    }
}
