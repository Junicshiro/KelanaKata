using UnityEngine;
using System.Collections.Generic;

public class ItemObjek : MonoBehaviour
{
    [Header("Identitas Kamus")]
    public string kataIndonesia;
    public string kataTobelo;
    // Di dalam ItemObjek.cs tambahkan:
    public bool isBattleItem; // Centang ini di Inspector untuk item kosakata stage
    // Di dalam ItemObjek.cs tambahkan:
    public AudioClip suaraTobelo; // Masukkan file .mp3 / .wav di Inspector

    [Header("Pengaturan Objek")]
    public bool hancurSetelahDiambil = true; // True untuk benda kecil, False untuk benda besar
    
    [TextArea(2, 5)]
    public string deskripsiItem;

    [Header("Referensi Visual (Untuk Benda Besar)")]
    public GameObject visualBlink; // Seret objek anak (Visual_Blink) ke sini

    private bool sudahPernahDiinteraksi = false;

    [Header("Save Settings")]
    [Tooltip("Berikan nama/ID unik yang berbeda untuk setiap item di Map!")]
    public string itemID; 

    // List statis untuk menampung ID item yang sudah diambil selama game berjalan
    // List ini akan otomatis terhapus bersih dari memori saat game dimatikan/di-close
    private static List<string> daftarItemTerambilPermanen = new List<string>();

    private void Start()
    {
        // Membuat ID unik otomatis berdasarkan nama objek dan posisinya di map
        itemID = gameObject.name + "_" + transform.position.ToString();

        // CEK MEMORI: Jika ID item ini ada di dalam list statis, langsung matikan objeknya
        if (daftarItemTerambilPermanen.Contains(itemID))
        {
            gameObject.SetActive(false);
        }
    }

    public void Ambil()
    {
        if (sudahPernahDiinteraksi && !hancurSetelahDiambil)
        {
            DialogueManager.instance.StartDialogueFromItem(kataIndonesia, deskripsiItem);
            return;
        }

        InventoryManager.instance.TemukanTerjemahan(kataIndonesia, kataTobelo, suaraTobelo);
        
        // Pastikan ini dipanggil sebelum logika penghancuran
        DialogueManager.instance.StartDialogueFromItem(kataIndonesia, deskripsiItem);

        // SIMPAN STATUS: Tandai ID item ini sebagai "sudah diambil" (nilai 1)
        if (!daftarItemTerambilPermanen.Contains(itemID))
        {
            daftarItemTerambilPermanen.Add(itemID);
        }

        if (hancurSetelahDiambil)
        {
            // PERBAIKAN: Jangan langsung Destroy(gameObject)
            // Matikan visual agar terlihat seperti hilang, tapi script tetap hidup sebentar
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            
            // Matikan juga visual blink jika ada
            if (visualBlink != null) visualBlink.SetActive(false);

            // Hancurkan objek setelah jeda kecil (misal 2 detik) agar tidak menumpuk di memori
            // Destroy(gameObject, 2f); 
        }
        else
        {
            sudahPernahDiinteraksi = true;
            if (visualBlink != null)
            {
                Animator anim = visualBlink.GetComponent<Animator>();
                if (anim != null) anim.enabled = false;
            }
        }

        // Cek apakah ini item yang masuk hitungan stage
        if (isBattleItem) 
        {
            // Panggil StageManager (Wasit) untuk menghitung
            StageManager.instance.TambahKosakata(this.gameObject);
        }

        // Jalankan fungsi lama kamu (masuk inventory / dialog)
        gameObject.SetActive(false);
    }

    // Tambahkan ini di dalam script ItemObjek.cs
    public void MunculkanKembali()
    {
        // Hapus status simpanan karena player gagal dan harus ambil ulang
        if (daftarItemTerambilPermanen.Contains(itemID))
        {
            daftarItemTerambilPermanen.Remove(itemID);
        }
        
        // Aktifkan kembali objek utama
        gameObject.SetActive(true);

        // Cek apakah punya SpriteRenderer, jika ada baru aktifkan
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.enabled = true;
        }

        // Cek apakah punya Collider2D, jika ada baru aktifkan
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        if (visualBlink != null) 
        {
            visualBlink.SetActive(true);
            // Jika sebelumnya animasinya dimatikan, aktifkan lagi
            Animator anim = visualBlink.GetComponent<Animator>();
            if (anim != null) anim.enabled = true;
        }
        
        sudahPernahDiinteraksi = false;
    }

    // TAMBAHKAN FUNGSI INI DI DALAM SCRIPT ItemObjek.cs
    public static void ResetSemuaDataKosakata()
    {
        if (daftarItemTerambilPermanen != null)
        {
            daftarItemTerambilPermanen.Clear();
            Debug.Log("Memori Item Kosakata berhasil dibersihkan total!");
        }
    }

    // --- Sitem Deteksi (Gunakan yang sudah ada sebelumnya) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerInteraction>().SetItemDidekat(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerInteraction>().SetItemDidekat(null);
    }
}