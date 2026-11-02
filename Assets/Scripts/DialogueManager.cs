using UnityEngine;
using TMPro; 
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI namaText;
    public TextMeshProUGUI isiDialogText;
    public AudioSource sfxSource;    // Audio Source untuk SFX
    public AudioClip dialogOpenSound; // Drag file audio ke sini

    private Queue<string> sentences; // Antrean kalimat
    private NPCInteraksi npcAktif; // Menyimpan data NPC yang sedang bicara

[HideInInspector] // Menyembunyikan dari inspector agar tidak membingungkan, diatur lewat code
    public bool isPrologMode = false;

    // Antrean khusus untuk menampung baris dialog beserta nama pembicaranya
    private Queue<PrologAutoTrigger.BarisDialogProlog> prologSentences = new Queue<PrologAutoTrigger.BarisDialogProlog>();

    void Awake()
    {
        if (instance == null) instance = this;
        sentences = new Queue<string>();
    }

    public void StartDialogue(NPCInteraksi npc)
    {
        npcAktif = npc;
        dialoguePanel.SetActive(true);
        // MAINKAN SUARA (Cukup di sini saja)
        if (sfxSource != null && dialogOpenSound != null)
        {
            sfxSource.PlayOneShot(dialogOpenSound);
        }
        namaText.text = npc.namaNPC;

        sentences.Clear();

        // Masukkan semua baris dialog NPC ke dalam antrean
        foreach (string sentence in npc.barisDialog)
        {
            sentences.Enqueue(sentence);
        }

        Time.timeScale = 0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().enabled = false;

StopAllCoroutines();
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Jika antrean habis, tutup dialog
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (sfxSource != null && dialogOpenSound != null)
        {
            sfxSource.PlayOneShot(dialogOpenSound); // Ganti dengan dialogOpenSound jika ingin suaranya sama
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // Efek mengetik agar lebih estetik
    IEnumerator TypeSentence(string sentence)
    {
        // 1. Masukkan teks lengkap beserta tag-nya secara utuh
        isiDialogText.text = sentence;
        
        // 2. Sembunyikan semua karakter visual
        isiDialogText.maxVisibleCharacters = 0;
        isiDialogText.ForceMeshUpdate();

        int totalVisibleCharacters = isiDialogText.textInfo.characterCount;

        // 3. Ketik hanya jumlah karakter visualnya saja (Tag otomatis tersembunyi di background)
        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            isiDialogText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(0.02f); 
        }
    }

    void EndDialogue()
    {
        // 1. Kembalikan fungsi penutup dialog
        dialoguePanel.SetActive(false); // Pastikan ini disamakan jika sebelumnya namanya dialoguePanel atau panelDialog
        Time.timeScale = 1f;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.GetComponent<PlayerMovement>() != null)
        {
            player.GetComponent<PlayerMovement>().enabled = true;
        }

        Debug.Log("Dialog Selesai.");


        // Jika sedang mode prolog, jalankan sekuens perpindahan prolog
        if (isPrologMode)
        {
            isPrologMode = false; // Reset kembali ke false agar tidak nyangkut
            SelesaiDialog();      // Panggil fungsi pemindah scene prolog kamu
        }
    }
    public void StartDialogueFromItem(string judul, string deskripsi)
    {
        Time.timeScale = 0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().enabled = false;
        
        npcAktif = null; // Pastikan tidak ada data NPC yang nyangkut
        dialoguePanel.SetActive(true);
        // MAINKAN SUARA (Cukup di sini saja)
        if (sfxSource != null && dialogOpenSound != null)
        {
            sfxSource.PlayOneShot(dialogOpenSound);
        }
        namaText.text = judul; 
        
        sentences.Clear();
        sentences.Enqueue(deskripsi); 

        DisplayNextSentence();
    }

    // Di dalam DialogueManager.cs

    public void PicuKartuDariDialog(string idKosakata)
    {
        if (npcAktif != null)
        {
            if (idKosakata == npcAktif.idKosakata)
            {
                // Cek apakah kata ini sudah pernah disimpan di Kamus sebelumnya
                if (InventoryManager.instance != null)
                {
                    // Ambil kata Indonesia dari NPC saat ini
                    string kataIndo = npcAktif.kataIndonesia;

                    // Cek apakah key kata tersebut sudah ada di Dictionary
                    if (InventoryManager.instance.kamusTobelo.ContainsKey(kataIndo))
                    {
                        // Ambil status artinya saat ini
                        string statusTerjemahan = InventoryManager.instance.kamusTobelo[kataIndo].kataTobelo;

                        // Jika artinya BUKAN "???", berarti kata ini SUDAH PERNAH DIKLIK & DISIMPAN!
                        if (statusTerjemahan != "???")
                        {
                            Debug.Log("Kosakata '" + kataIndo + "' sudah dipelajari. Klik diabaikan agar tidak duplikat.");
                            return; // Keluar dari fungsi secara paksa, kartu tidak akan muncul lagi!
                        }
                    }
                }

                // Jika lolos pengecekan di atas (artinya masih "???"), baru munculkan kartu
                if (KartuKosakataManager.instance != null)
                {
                    KartuKosakataManager.instance.TampilkanKartu(
                        npcAktif.kataIndonesia,
                        npcAktif.kataTobelo,
                        npcAktif.suaraTobelo
                    );
                }
                else
                {
                    Debug.LogError("KartuKosakataManager tidak ditemukan di Scene!");
                }
            }
            else
            {
                Debug.LogWarning("ID Link '" + idKosakata + "' tidak cocok dengan ID Kosakata NPC ini.");
            }
        }
    }

    // Contoh modifikasi di DialogueManager.cs saat teks habis
    public void SelesaiDialog()
    {
        dialoguePanel.SetActive(false);

        // Cek di mana kita sekarang berdasarkan nama Scene
        string sceneSekarang = SceneManager.GetActiveScene().name;

        if (sceneSekarang == "PrologDesa")
        {
            // Jika di luar rumah selesai bicara, langsung pindah ke dalam rumah
            PrologController.instance.namaSceneBerikutnya = "PrologDalamRumah";
            PrologController.instance.PindahScene();
        }
        else if (sceneSekarang == "PrologDalamRumah")
        {
            // Jika di dalam rumah selesai bicara, player jalan otomatis ke tangga
            PrologController.instance.PerintahKeLoteng();
        }
        else if (sceneSekarang == "PrologLoteng")
        {
            // Jika dialog buku di loteng selesai, masuk ke Stage 1 (Game Utama)
            SceneManager.LoadScene("DalamRumah");
        }
    }


    // Fungsi baru untuk memulai dialog multi-karakter khusus prolog
    public void StartPrologMultiKarakter(List<PrologAutoTrigger.BarisDialogProlog> listDialog)
    {
        dialoguePanel.SetActive(true);
        
        if (sfxSource != null && dialogOpenSound != null)
        {
            sfxSource.PlayOneShot(dialogOpenSound);
        }

        prologSentences.Clear();

        // Masukkan seluruh list ke dalam antrean khusus prolog
        foreach (var baris in listDialog)
        {
            prologSentences.Enqueue(baris);
        }

        Time.timeScale = 0f;

        // Panggil fungsi cetak kalimat khusus prolog
        TampilkanKalimatPrologBerikutnya();
    }

    public void TampilkanKalimatPrologBerikutnya()
    {
        // Jika antrean prolog habis, tutup dialog lewat EndDialogue bawaanmu
        if (prologSentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (sfxSource != null && dialogOpenSound != null)
        {
            sfxSource.PlayOneShot(dialogOpenSound); // Ganti dengan dialogOpenSound jika ingin suaranya sama
        }

        // Ambil data baris dialog terdepan
        PrologAutoTrigger.BarisDialogProlog barisSekarang = prologSentences.Dequeue();

        // UBAH NAMA DI UI SESUAI DENGAN SIAPA YANG BICARA DI BARIS INI
        namaText.text = barisSekarang.namaKarakter;

        // Jalankan efek mengetik kalimatnya
        StopAllCoroutines();
        StartCoroutine(TypeSentence(barisSekarang.kalimat));
    }

    public void BukaKosakataLewatPanel()
    {
        // 1. Jika teks dialog sedang berjalan (belum selesai mengetik), kita abaikan dulu
        // atau jika data NPC aktif ternyata kosong, langsung keluar.
        if (npcAktif == null) return;

        // 2. Ambil ID Kosakata langsung dari NPC yang saat ini sedang aktif berbicara
        string idKosakataNPC = npcAktif.idKosakata;

        // Jika NPC ini memang memiliki ID kosakata (tidak kosong), langsung picu kartunya!
        if (!string.IsNullOrEmpty(idKosakataNPC))
        {
            Debug.Log($"Panel Dialog diklik! Mengambil kosakata dari NPC: {npcAktif.namaNPC}");
            
            // Panggil fungsi pembuka kartu bawaanmu yang sudah ada di atas
            PicuKartuDariDialog(idKosakataNPC);
        }
    }

}

