using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioClip[] footstepClips;
    public float stepCooldown = 0.3f;

    private AudioSource audioSource;
    private float lastStepTime = 0f;

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
        if (footstepClips.Length > 0 && audioSource != null && Time.time - lastStepTime >= stepCooldown)
        {
            lastStepTime = Time.time;

            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            Debug.Log("Playing footstep clip: " + clip.name);

            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
