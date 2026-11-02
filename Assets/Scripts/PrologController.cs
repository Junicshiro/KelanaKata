using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PrologController : MonoBehaviour
{
    public static PrologController instance;

    [Header("Settings Pergerakan")]
    public GameObject playerObjek;
    public Animator playerAnim;
    public Transform titikTangga; // Tarik objek TitikTangga ke sini
    public float kecepatanJalan = 2f;

    [Header("UI & Transisi")]
    public string namaSceneBerikutnya; // Nama scene dalam rumah atau loteng

    private bool sedangJalanKeTangga = false;

    void Awake()
    {
        instance = this;
    }

    // --- FUNGSI UNTUK FASE 1 (DI LUAR RUMAH) ---
    public void MulaiPrologLuar()
    {
        // Panggil dialog pertama melalui DialogueManager kamu
        // Contoh: DialogueManager.instance.MulaiDialog("PrologLuar");
    }

    // --- FUNGSI UNTUK FASE 2 (DI DALAM RUMAH) ---
    public void PerintahKeLoteng()
    {
        // Fungsi ini dipanggil setelah dialog Ibu selesai
        StartCoroutine(PlayerJalanKeTangga());
    }

    IEnumerator PlayerJalanKeTangga()
    {
        sedangJalanKeTangga = true;
        
        // Matikan script pergerakan manual player
        PlayerMovement pergerakanPlayer = playerObjek.GetComponent<PlayerMovement>();
        if (pergerakanPlayer != null) pergerakanPlayer.enabled = false;

        // Hitung Arah Menuju Tangga untuk Animator
        Vector2 posisiPlayer = playerObjek.transform.position;
        Vector2 posisiTangga = titikTangga.position;
        
        // Hitung arah (arah = posisi tujuan - posisi sekarang, lalu dinormalisasi)
        Vector2 arahJalan = (posisiTangga - posisiPlayer).normalized;

        if (playerAnim != null) 
        {
            // Set parameter arah di Animator kamu (Sesuaikan namanya jika berbeda, misal: moveX/moveY)
            // Kita gunakan arahJalan.x untuk Horizontal, dan arahJalan.y untuk Vertical
            playerAnim.SetFloat("Horizontal", arahJalan.x);
            playerAnim.SetFloat("Vertical", arahJalan.y);
            
            // Nyalakan tanda sedang berjalan (jika parameternya boolean)
            playerAnim.SetBool("IsMoving", true); 
            
            // Jika parameternya float Speed (alternatif)
            playerAnim.SetFloat("Speed", 1f); 
            
            Debug.Log("[Animator Log] Mengatur arah jalan otomatis ke: X=" + arahJalan.x + ", Y=" + arahJalan.y);
        }

        // Loop pergerakan otomatis seperti biasa
        while (Vector2.Distance(playerObjek.transform.position, titikTangga.position) > 0.1f)
        {
            playerObjek.transform.position = Vector2.MoveTowards(
                playerObjek.transform.position, 
                titikTangga.position, 
                kecepatanJalan * Time.deltaTime
            );
            yield return null;
        }

        // Sampai di tangga, matikan animasi
        if (playerAnim != null) 
        {
            playerAnim.SetBool("IsMoving", false);
            playerAnim.SetFloat("Speed", 0f);
            
            // Reset parameter arah saat berhenti agar tidak 'nyangkut' menghadap kanan terus
            // playerAnim.SetFloat("Horizontal", 0f);
            // playerAnim.SetFloat("Vertical", 0f);
        }
        
        sedangJalanKeTangga = false;
        PindahScene();
    }

    IEnumerator ProsesPindahScene()
    {
        if (FadeManager.instance != null)
        {
            // Jalankan fade TANPA kata 'yield return' di depannya agar script tidak terkunci menunggu
            StartCoroutine(FadeManager.instance.StartFadeInOut());
        }

        // atur timing fade
        yield return new WaitForSeconds(1.2f); 

        // Pindah scene tepat saat layar sedang hitam pekat!
        UnityEngine.SceneManagement.SceneManager.LoadScene(namaSceneBerikutnya);
    }

    public void PindahScene()
    {
        // Gunakan FadeManager kamu jika ada
        if (FadeManager.instance != null)
        {
            StartCoroutine(ProsesPindahScene());
        }
        else
        {
            SceneManager.LoadScene(namaSceneBerikutnya);
        }
    }
}