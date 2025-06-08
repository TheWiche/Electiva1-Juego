using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    public AudioClip[] footstepClips;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on " + gameObject.name);
        }
    }
    
    public void PlayFootstep()
    {
        if (footstepClips.Length > 0 && audioSource != null)
        {
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}
