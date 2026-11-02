using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    
    [Header("Pengaturan Fade")]
    public CanvasGroup canvasGroup; // Pastikan di Inspector objek CanvasGroup ini ditarik!
    public float fadeSpeed = 1f;

    void Awake()
    {
        // PENTING: Awake harus selalu bersih dan berada di atas untuk inisialisasi awal Singleton
        if (instance == null)
        {
            instance = this;
            // Jika FadeManager kamu dipasang DontDestroyOnLoad, aktifkan baris di bawah ini:
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Jalankan saat pertama kali game dimulai di scene pertama
        MulaiFadeDariGelapKeTerang();
    }

    // --- FUNGSI UNTUK KELUAR BATTLE / GAGAL STAGE) ---
    public IEnumerator StartFadeInOut()
    {
        // 1. FADE OUT (Menghitam)
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        // --- DI TITIK INI LAYAR HITAM TOTAL ---
        if (StageManager.instance != null)
        {
            StageManager.instance.GagalBattle();
        }

        yield return new WaitForSeconds(0.5f); // Jeda sebentar saat gelap

        // 2. FADE IN (Normal Kembali)
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
    }

    // PERBAIKAN UTAMA: FUNGSI OTOMATIS MENERANG SAAT MASUK SCENE BARU
    public void MulaiFadeDariGelapKeTerang()
    {
        // PERBAIKAN: Langsung gunakan 'canvasGroup' global dari Inspector, tidak perlu GetComponent ulang yang bikin error
        if (canvasGroup != null)
        {
            // Paksa layar menjadi hitam pekat terlebih dahulu saat awal scene masuk
            canvasGroup.alpha = 1f;
            
            // Hentikan coroutine fade terang yang lama jika ada yang sedang berjalan agar tidak tabrakan
            StopCoroutine("ProsesFadeTerang");
            
            // Jalankan proses memudar ke terang secara halus
            StartCoroutine(ProsesFadeTerang(canvasGroup));
        }
        else
        {
            Debug.LogError("[FadeManager Error] Variabel 'canvasGroup' di Inspector kosong! Tolong drag objek Panel Hitam ke slot ini.");
        }
    }

    private IEnumerator ProsesFadeTerang(CanvasGroup cg)
    {
        float counter = 0f;
        float durasiFade = 1f; // Ubah angka ini (detik) jika ingin transisinya lebih lambat/cepat

        while (counter < durasiFade)
        {
            counter += Time.deltaTime;
            // Perlahan menurunkan alpha dari 1 (hitam) menuju 0 (terang/transparan)
            cg.alpha = Mathf.Lerp(1f, 0f, counter / durasiFade);
            yield return null;
        }

        cg.alpha = 0f; // Pastikan transparan sempurna setelah selesai
    }

    // Mengunci event listener scene di Unity
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Otomatis berjalan secara ajaib setiap kali scene baru sukses terbuka
        MulaiFadeDariGelapKeTerang();
    }
}