using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MekanismePintuLuar : MonoBehaviour
{
    [Header("Konfigurasi Scene")]
    public string namaSceneTujuan;
    public bool iniPintuKeluarScene = false; // Centang jika ini pintu tempat player MUNCUL di scene ini

    [Header("Komponen")]
    public Animator pintuAnim;
    public float kecepatanJalan = 2f;

    private bool sedangProses = false;
    private bool playerDekat = false;

    void Start()
    {
        // Cek apakah variabel static di PortalScene bernilai true
    if (iniPintuKeluarScene && PortalScene.sedangPindahDariDalam)
    {
        StartCoroutine(SequenceMunculDariPintu());
        
        // JANGAN LUPA: Set kembali ke false agar tidak terulang terus
        PortalScene.sedangPindahDariDalam = false;
    }
    }

    // void Update()
    // {
    //     // Logika untuk MASUK ke dalam pintu
    //     if (playerDekat && Input.GetKeyDown(KeyCode.E) && !sedangProses)
    //     {
    //         StartCoroutine(SequenceMasukKePintu());
    //     }
    // }

    public void InteraksiPintuTransisi()
    {
        if (playerDekat && !sedangProses)
        {
            StartCoroutine(SequenceMasukKePintu());
        }
    }

    // --- URUTAN 1: MASUK KE PINTU (Seperti masuk rumah) ---
    IEnumerator SequenceMasukKePintu()
    {
        sedangProses = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Animator playerAnim = player.GetComponent<Animator>();
        
        // 1. Matikan kontrol player agar tidak bisa digerakkan user
        player.GetComponent<PlayerMovement>().enabled = false;

        // 2. Buka Pintu
        pintuAnim.SetBool("isOpen", true);
        yield return new WaitForSeconds(0.3f);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if(rb != null) rb.bodyType = RigidbodyType2D.Kinematic; // Matikan fisika sementara
        
        // 2. SET ANIMASI JALAN (Ke arah Atas)
    if (playerAnim != null)
    {
        playerAnim.SetBool("IsMoving", true); // Player mulai bergerak
        playerAnim.SetFloat("Speed", 2f);      // Set speed (asumsi nilai 2 adalah jalan)
        playerAnim.SetFloat("Horizontal", 0); // Menghadap lurus
        playerAnim.SetFloat("Vertical", 1);   // Menghadap Atas
    }
        
        // 3. Player gerak otomatis ke atas (masuk)
        Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y + 0.5f, 0);
        while (Vector3.Distance(player.transform.position, targetPos) > 0.05f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, kecepatanJalan * Time.deltaTime);
            yield return null;
        }

        // 3. SET IDLE sebelum hilang
    if (playerAnim != null) playerAnim.SetBool("IsMoving", false);

        // 4. Player Hilang & Pintu Tutup
        player.GetComponent<SpriteRenderer>().enabled = false;
        pintuAnim.SetBool("isOpen", false);
        yield return new WaitForSeconds(0.6f);

        // 5. Pindah Scene
        SceneManager.LoadScene(namaSceneTujuan);
    }

    // --- URUTAN 2: MUNCUL DARI PINTU (Saat baru pindah scene) ---
    IEnumerator SequenceMunculDariPintu()
    {
        sedangProses = true;
        
        // Tunggu sampai sistem benar-benar siap (satu frame)
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Animator playerAnim = player.GetComponent<Animator>();
            // 1. Persiapan Awal
            // Pindahkan player ke posisi pintu (agak ke atas sedikit agar terlihat keluar dari dalam)
            player.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, 0);
            
            // Pastikan player tidak bisa digerakkan dulu
            player.GetComponent<PlayerMovement>().enabled = false;
            
            player.GetComponent<SpriteRenderer>().enabled = false;
            pintuAnim.SetBool("isOpen", true);
            
            yield return new WaitForSeconds(0.5f); // Beri jeda pintu terbuka

// 2. SET ANIMASI JALAN (Ke arah Atas)
    if (playerAnim != null)
    {
        playerAnim.SetBool("IsMoving", true); // Player mulai bergerak
        playerAnim.SetFloat("Speed", 2f);      // Set speed (asumsi nilai 2 adalah jalan)
        playerAnim.SetFloat("Horizontal", 0); // Menghadap lurus
        playerAnim.SetFloat("Vertical", -1);   // Menghadap Atas
    }

            // 2. Munculkan player & Gerak Turun
            player.GetComponent<SpriteRenderer>().enabled = true;
            
            // Target posisi di depan pintu (y dikurangi agar turun)
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y - 0.5f, 0);
            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if(rb != null) rb.bodyType = RigidbodyType2D.Kinematic; // Matikan fisika sementara
            while (Vector3.Distance(player.transform.position, targetPos) > 0.05f)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, kecepatanJalan * Time.deltaTime);
                yield return null;
            }

// 3. SET IDLE sebelum hilang
    if (playerAnim != null) playerAnim.SetBool("IsMoving", false);

            // 3. Kembalikan ke Dynamic agar player bisa menabrak benda lagi
            if(rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
            
            pintuAnim.SetBool("isOpen", false);

            // 4. Nyalakan kembali script movement
            player.GetComponent<PlayerMovement>().enabled = true;
        }
        else
        {
            Debug.LogError("Player tidak ditemukan! Pastikan hantu kamu punya Tag 'Player'");
        }

        sedangProses = false;
    }

    // --- DETEKSI AREA (Diupdate agar lapor ke PlayerInteraction) ---
    private void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) 
        {
            playerDekat = true;
            other.GetComponent<PlayerInteraction>().SetPintuTransisiDidekat(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) 
        {
            playerDekat = false;
            other.GetComponent<PlayerInteraction>().SetPintuTransisiDidekat(null);
        }
    }
}