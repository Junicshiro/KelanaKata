using UnityEngine;

public class DoorSFX : MonoBehaviour
{
    public AudioSource sfxSource;    // Audio Source untuk SFX
    public AudioClip soundBuka;
    public AudioClip soundTutup;

    // void Awake()
    // {
    //     audioSource = GetComponent<AudioSource>();
    // }

    // Fungsi ini akan dipanggil oleh Animation Event
    public void PlaySoundBuka()
    {
        sfxSource.PlayOneShot(soundBuka);
    }

    public void PlaySoundTutup()
    {
        sfxSource.PlayOneShot(soundTutup);
    }
}