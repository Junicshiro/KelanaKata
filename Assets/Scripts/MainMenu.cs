using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pengaturanPanel; // Panel Induk
    public GameObject panelAudio;      // Sub-panel Audio
    public GameObject panelLayout;     // Sub-panel Layout

    [Header("Audio")]
    public AudioSource uiSource;
    public AudioClip soundOpen;
    public AudioClip soundClick;

    void Awake()
    {
        // PENGAMANAN: Setiap kali scene tempat script ini menempel aktif,
        // kita cek apakah ini scene Main Menu.
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            // Memanggil fungsi reset statis yang ada di ItemObjek
            ItemObjek.ResetSemuaDataKosakata();
        }
    }

    public void MulaiGame()
    {
        // Sebelum pindah ke Stage 1, pastikan sekali lagi data bersih
        ItemObjek.ResetSemuaDataKosakata();
        uiSource.PlayOneShot(soundOpen);
        SceneManager.LoadScene("PrologDesa");
        Time.timeScale = 1f;
        CustomControl.bolehEditLayout = false;
        Debug.Log("Mode Edit Layout: OFF. Posisi UI dikunci.");
    }

    // Dipanggil saat klik tombol "Pengaturan" di Main Menu
    public void PengaturanGame()
    {
        uiSource.PlayOneShot(soundOpen);
        pengaturanPanel.SetActive(true);
        
        // Default saat buka pengaturan: Audio muncul, Layout sembunyi
        BukaPanelAudio();
    }

    // Fungsi untuk tombol "Setting Audio" atau saat pertama buka pengaturan
    public void BukaPanelAudio()
    {
        uiSource.PlayOneShot(soundClick);
        panelAudio.SetActive(true);
        panelLayout.SetActive(false);
    }

    // Fungsi untuk tombol yang akan membuka Custom Layout
    public void BukaPanelLayout()
    {
        uiSource.PlayOneShot(soundClick);
        panelAudio.SetActive(false);
        panelLayout.SetActive(true);

        CustomControl.bolehEditLayout = true;
        Debug.Log("Mode Edit Layout: ON. UI sekarang bisa digeser.");
    }

    public void KembaliKeMainMenu()
    {
        uiSource.PlayOneShot(soundOpen);
        pengaturanPanel.SetActive(false);
    }

    public void KeluarGame()
    {
        uiSource.PlayOneShot(soundOpen);
        Debug.Log("Game Berhasil Keluar");
        Application.Quit();
    }

    public void PlayClickSound()
    {
        uiSource.PlayOneShot(soundClick);
    }

    // void Awake()
    // {
    //     // Reset data layout (untuk development)
    //     PlayerPrefs.DeleteKey("Joystick_X");
    //     PlayerPrefs.DeleteKey("Joystick_Y");
    //     PlayerPrefs.DeleteKey("ButtonA_X");
    //     PlayerPrefs.DeleteKey("ButtonA_Y");
    //     PlayerPrefs.DeleteKey("ButtonB_X");
    //     PlayerPrefs.DeleteKey("ButtonB_Y");
    //     Debug.Log("Data Layout Sementara Direset!");
    // }
}