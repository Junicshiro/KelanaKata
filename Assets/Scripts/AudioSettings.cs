using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer mainMixer;
    
    [Header("Slider References")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Load nilai volume yang tersimpan, jika tidak ada gunakan 0.75
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        bgmSlider.value = savedBGM;
        sfxSlider.value = savedSFX;

        SetBGMVolume(savedBGM);
        SetSFXVolume(savedSFX);
    }

    public void SetBGMVolume(float value)
    {
        mainMixer.SetFloat("BGMParam", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        float volume = Mathf.Log10(value) * 20;
        bool success = mainMixer.SetFloat("SFXParam", volume);
        
        Debug.Log("Mengatur Volume SFX ke: " + volume + " | Berhasil: " + success);
        
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}