using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource source;
    public AudioClip dot;
    public AudioClip powerUp;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayPowerUpSound()
    {
        source.clip = powerUp;
        source.PlayOneShot(powerUp);
    }

    public void PlayDotSound()
    {
        source.clip = dot;
        source.Play();
    }
}
