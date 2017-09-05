using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Sounds_and_Particles : MonoBehaviour
{
    public AudioClip openClip;
    public AudioClip closeClip;
    public AudioClip poisonClip;
    public AudioClip yayClip;
    public AudioClip dustClip;

    private AudioSource audioSource;
    private Animator anim;
    private AnimatorStateInfo oldStateInfo;
    public ParticleSystem chestParticle;

    enum PlayClip
    {
        NULL,
        OPEN,
        CLOSE
    }

    private PlayClip playClip;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        chestParticle = GetComponentInChildren<ParticleSystem>();
        if (chestParticle == null)
        {
            print("Particle system is not present on Game object:" + name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        playClip = PlayClip.NULL;
        if (stateInfo.IsName("open"))
        {
            // Changing the audio clip to open
            audioSource.clip = openClip;
            playClip = PlayClip.OPEN;
            //audioSource.Play();
        }
        else if(stateInfo.IsName("close"))
        {
            audioSource.clip = closeClip;
            playClip = PlayClip.CLOSE;
            //audioSource.Play();
        }

        if (oldStateInfo.shortNameHash != stateInfo.shortNameHash && playClip != PlayClip.NULL)
        {
            audioSource.Play();
            if (chestParticle != null && playClip == PlayClip.OPEN)
            {
                chestParticle.Play();
            }
        }

        // Store previous frame's state info
        oldStateInfo = stateInfo;
    }
}
