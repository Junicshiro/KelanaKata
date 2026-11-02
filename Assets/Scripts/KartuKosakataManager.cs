using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KartuKosakataManager : MonoBehaviour
{
    public static KartuKosakataManager instance;

    [Header("Referensi UI Kartu")]
    public GameObject panelKartu;
    public TextMeshProUGUI txtTobelo;
    public TextMeshProUGUI txtIndonesia;
    public bool isBattleItem; // Centang ini di Inspector untuk item kosakata stage
    // public TextMeshProUGUI txtDeskripsi;
    public AudioSource audioSource;

    private AudioClip suaraAktif;
    private string namaIndoAktif;
    private string kataTobeloAktif;

    void Awake()
    {
        instance = this;
        if(panelKartu != null) panelKartu.SetActive(false);
    }

    // Fungsi yang dipanggil saat kata di dialog diklik
    public void TampilkanKartu(string namaIndo, string kataTobelo, AudioClip suara)
    {
        namaIndoAktif = namaIndo;
        kataTobeloAktif = kataTobelo;
        suaraAktif = suara;

        // Isi data ke UI
        txtTobelo.text = kataTobelo;
        txtIndonesia.text = namaIndo;
        // txtDeskripsi.text = deskripsi;

        // Munculkan panel kartu dan hentikan waktu game sebentar (Pause)
        panelKartu.SetActive(true);
        // Time.timeScale = 0f; 

        // Otomatis putar suara sekali saat kartu muncul
        // PutarSuara();
    }

    public void PutarSuara()
    {
        if (audioSource != null && suaraAktif != null)
        {
            audioSource.PlayOneShot(suaraAktif);
        }
    }

    // Fungsi untuk tombol "Pahami & Simpan"
    public void TombolSimpanDanTutup()
    {
        /// 1. Tetap simpan ke Inventory permanen
    if (InventoryManager.instance != null)
    {
        // Tambahkan parameter suaraAktif di bagian belakang
        InventoryManager.instance.TemukanTerjemahan(namaIndoAktif, kataTobeloAktif, suaraAktif);
    }

    // 2. TRIK SELESAIKAN MASALAH STAGEMANAGER
    if (StageManager.instance != null)
    {
        // Buat sebuah GameObject kosong tak terlihat di memori RAM
        GameObject objekTiruan = new GameObject("KosakataNPC_" + kataTobeloAktif);
        
        // Tempelkan script ItemObjek secara paksa ke objek tiruan tersebut
        ItemObjek itemTiruan = objekTiruan.AddComponent<ItemObjek>();
        
        // Isi data item tiruan ini dengan data kosakata yang berasal dari NPC tadi
        itemTiruan.kataIndonesia = namaIndoAktif;
        itemTiruan.kataTobelo = kataTobeloAktif;
        // itemTiruan.isBattleItem = true; // Set true agar dihitung sebagai item stage oleh StageManager
        itemTiruan.suaraTobelo = suaraAktif;

        // Kirim objek tiruan yang sudah menyamar jadi "ItemObjek" ini ke StageManager
        StageManager.instance.TambahKosakata(objekTiruan);
        
        // Catatan: Biasanya setelah StageManager memproses objek ini (mengambil datanya untuk dimasukkan ke list kamus),
        // objekTiruan ini bisa langsung dihancurkan atau dibiarkan tergantung apakah StageManager kamu 
        // langsung menghancurkannya atau tidak di dalam fungsinya.
    }

        // Tutup kartu dan jalankan waktu game kembali
        panelKartu.SetActive(false);
        // Time.timeScale = 1f; 
    }
}