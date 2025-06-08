using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    public AudioClip footstepClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayFootstep()
    {
        if (footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }
}
