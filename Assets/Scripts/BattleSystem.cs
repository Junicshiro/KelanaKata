using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; 

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem instance;

    [Header("UI References")]
    public GameObject battlePanelUI;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons; 
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI monsterHPText;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("Efek Feedback Kuis")]
    public FeedbackEfek efekPlayer;
    public FeedbackEfek efekMonster;

    [Header("Pengaturan Efek")]
    public float durasiEfek = 0.4f;      // durasi efek
    public float kekuatanGuncang = 0.15f; // Seberapa heboh guncangannya

    private int currentHPPlayer;
    private int currentHPMonster;
    private ItemObjek correctItem; // Data item yang menjadi jawaban benar

    private List<GameObject> antreanSoal = new List<GameObject>();

    // Tambahkan variabel ini di bagian [Header("UI References")] atau di bawahnya
    [Header("Pengaturan Boss")]
    public bool isFinalBoss = false; // Centang ini di Inspector khusus di Scene Final Boss!

    // List baru untuk menampung data nama Indonesia dari kosakata global (Khusus Final Boss)
    private List<string> antreanSoalFinalBoss = new List<string>();
    private string correctKataIndoFinalBoss; // Menyimpan kunci jawaban berupa string untuk Final Boss

    [Header("Pengaturan Tamat")]
    public GameObject panelTamatUI;
    public float jedaSebelumTamatMuncul = 1.5f; // Beri waktu agar efek monster kalah selesai (detik)
    public float kecepatanMunculTamat = 2f;

    [Header("Pengaturan Warna Feedback")]
    public Color warnaBenar = Color.green;
    public Color warnaSalah = Color.red;
    public Color warnaNormal = Color.white;

    void Awake() 
    { 
        instance = this; 
    }

    // public void PersiapkanBattle(int jumlahKosakata)
    // {
    //     currentHPPlayer = jumlahKosakata;
    //     currentHPMonster = jumlahKosakata;

    //     // Bersihkan antrean lama, lalu isi ulang dengan menyaring objek yang valid (tidak null)
    //     antreanSoal.Clear();
    //     foreach (GameObject obj in StageManager.instance.listItemTerambil)
    //     {
    //         if (obj != null && obj.GetComponent<ItemObjek>() != null)
    //         {
    //             antreanSoal.Add(obj);
    //         }
    //     }

    //     UpdateHPUI();
    // }

    public void MulaiBattle()
    {
        battlePanelUI.SetActive(true);
        LoadPertanyaan();
    }

    void LoadPertanyaan()
    {
        List<string> answers = new List<string>();
        string kataIndonesiaSoal = "";
        string kataTobeloBenar = "";

        // JALUR 1: LOGIKA PEMILIHAN SOAL UNTUK FINAL BOSS (SEMUA STAGE)
        if (isFinalBoss)
        {
            if (antreanSoalFinalBoss.Count == 0)
            {
                IsiUlangAntreanSoal();
                Debug.Log("Semua soal Final Boss sudah muncul, mengulang antrean...");
            }

            int randomIndex = Random.Range(0, antreanSoalFinalBoss.Count);
            kataIndonesiaSoal = antreanSoalFinalBoss[randomIndex];
            
            // Ambil jawaban benarnya dari InventoryManager
            kataTobeloBenar = InventoryManager.instance.kamusTobelo[kataIndonesiaSoal].kataTobelo;
            antreanSoalFinalBoss.RemoveAt(randomIndex);

            questionText.text = " Apa bahasa Tobelo dari: " + kataIndonesiaSoal + "?";
            answers.Add(kataTobeloBenar);

            // Kumpulkan pilihan jawaban salah dari database inventory global yang sudah terbuka
            List<string> poolSalahFinalBoss = new List<string>();
            foreach (var item in InventoryManager.instance.kamusTobelo)
            {
                if (item.Value.kataTobelo != "???" && item.Value.kataTobelo != kataTobeloBenar)
                {
                    poolSalahFinalBoss.Add(item.Value.kataTobelo);
                }
            }

            // Ambil pilihan salah acak sampai tombol penuh
            while (answers.Count < optionButtons.Length && poolSalahFinalBoss.Count > 0)
            {
                int r = Random.Range(0, poolSalahFinalBoss.Count);
                if (!answers.Contains(poolSalahFinalBoss[r])) answers.Add(poolSalahFinalBoss[r]);
                poolSalahFinalBoss.RemoveAt(r);
            }
            
            // Simpan ke variabel temporary string khusus pengecekan jawaban final boss
            correctKataIndoFinalBoss = kataIndonesiaSoal; 
        }

        // JALUR 2: LOGIKA PEMILIHAN SOAL UNTUK MONSTER BIASA (LOKAL STAGE) - KODE LAMA KAMU
        else
        {
            if (antreanSoal.Count == 0)
            {
                IsiUlangAntreanSoal();
                Debug.Log("Semua soal sudah muncul, mengulang antrean...");
            }

            int randomIndex = Random.Range(0, antreanSoal.Count);
            correctItem = antreanSoal[randomIndex].GetComponent<ItemObjek>();
            antreanSoal.RemoveAt(randomIndex);

            kataIndonesiaSoal = correctItem.kataIndonesia;
            kataTobeloBenar = correctItem.kataTobelo;

            questionText.text = "Apa bahasa Tobelo dari: " + kataIndonesiaSoal + "?";
            answers.Add(kataTobeloBenar);

            List<GameObject> semuaItem = StageManager.instance.listItemTerambil;
            List<GameObject> poolSalah = semuaItem.Where(x => x != null && 
                                                            x.GetComponent<ItemObjek>() != null && 
                                                            x.GetComponent<ItemObjek>().kataTobelo != kataTobeloBenar).ToList();

            while (answers.Count < optionButtons.Length && poolSalah.Count > 0)
            {
                int r = Random.Range(0, poolSalah.Count);
                string salah = poolSalah[r].GetComponent<ItemObjek>().kataTobelo;
                if (!answers.Contains(salah)) answers.Add(salah);
                poolSalah.RemoveAt(r);
            }
        }


        // BAGIAN PENGACAKAN TOMBOL DAN INTEGRASI UI (BERLAKU UNTUK KEDUA JALUR)
        for (int i = 0; i < answers.Count; i++) {
            string temp = answers[i];
            int r = Random.Range(i, answers.Count);
            answers[i] = answers[r];
            answers[r] = temp;
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < answers.Count) {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[i];
                
                // 🌟 TAMBAHKAN DUA BARIS INI:
                optionButtons[i].GetComponent<Image>().color = warnaNormal;
                optionButtons[i].interactable = true;

                string choice = answers[i];
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => CekJawaban(choice));
            } else {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void CekJawaban(string jawaban)
    {
        // Alihkan logika ke Coroutine agar bisa diberi jeda waktu warna
        StartCoroutine(ProsesEfekWarnaJawaban(jawaban));
    }

    // Fungsi baru untuk mengatur sekuens warna dan jeda kuis
    System.Collections.IEnumerator ProsesEfekWarnaJawaban(string jawaban)
    {
        // 1. Kunci semua tombol agar tidak bisa di-spam klik
        foreach (Button btn in optionButtons)
        {
            if (btn != null) btn.interactable = false;
        }

        // 2. Cari tahu kunci jawaban asli yang benar
        string targetJawabanBenar = "";
        if (isFinalBoss)
        {
            targetJawabanBenar = InventoryManager.instance.kamusTobelo[correctKataIndoFinalBoss].kataTobelo;
        }
        else
        {
            targetJawabanBenar = correctItem.kataTobelo;
        }

        // 3. Warnai tombol kuis secara real-time
        foreach (Button btn in optionButtons)
        {
            if (btn != null && btn.gameObject.activeSelf)
            {
                string teksTombol = btn.GetComponentInChildren<TextMeshProUGUI>().text;
                
                if (teksTombol == targetJawabanBenar)
                {
                    btn.GetComponent<Image>().color = warnaBenar; // Benar otomatis HIJAU
                }
                else if (teksTombol == jawaban)
                {
                    btn.GetComponent<Image>().color = warnaSalah; // Pilihan salah siswa otomatis MERAH
                }
            }
        }

        // 4. Mainkan audio feedback & guncangan bawaan kode asli kamu
        if (jawaban == targetJawabanBenar) 
        {
            Debug.Log("Jawaban Benar!");
            sfxSource.PlayOneShot(correctSound);
            if (efekMonster != null) efekMonster.MainkanEfek(durasiEfek, kekuatanGuncang);
        } 
        else 
        {
            Debug.Log("Jawaban Salah!");
            sfxSource.PlayOneShot(wrongSound);
            if (efekPlayer != null) efekPlayer.MainkanEfek(durasiEfek, kekuatanGuncang);
        }

        // 5. Beri jeda 1.5 detik agar warna sempat terlihat oleh siswa
        yield return new WaitForSeconds(1.5f);

        // 6. Lanjutkan ke pemotongan HP & antrean kuis asli kamu
        if (jawaban == targetJawabanBenar) 
        {
            KurangiHPMonster();
        } 
        else 
        {
            KurangiHPPlayer();

            if (isFinalBoss)
            {
                if (!antreanSoalFinalBoss.Contains(correctKataIndoFinalBoss))
                {
                    antreanSoalFinalBoss.Add(correctKataIndoFinalBoss);
                }
            }
            else
            {
                if (correctItem != null && !antreanSoal.Contains(correctItem.gameObject))
                {
                    antreanSoal.Add(correctItem.gameObject);
                }
            }
        }

        // 7. Muat soal berikutnya jika kedua pihak masih hidup
        if (currentHPMonster > 0 && currentHPPlayer > 0) 
        {
            LoadPertanyaan();
        }
    }

    // public void UpdateHPUI()
    // {
    //     playerHPText.text = "x" + currentHPPlayer.ToString();
    //     monsterHPText.text = "x" + currentHPMonster.ToString();
    // }

    public void KurangiHPMonster()
    {
        currentHPMonster--;
        UpdateHPUI();
        if (currentHPMonster <= 0) MenangBattle();
    }

    public void KurangiHPPlayer()
    {
        currentHPPlayer--;
        UpdateHPUI();
        if (currentHPPlayer <= 0) KalahBattle();
    }

    void MenangBattle()
    {
        battlePanelUI.SetActive(false);
        if (isFinalBoss)
        {
            Debug.Log("Final Boss telah dikalahkan! Memulai sekuens ending...");
            StartCoroutine(SekuensGameTamat());
        }
  
        // JIKA MONSTER BIASA: JALANKAN ALUR SEPERTI BIASA
        else
        {
            StageManager.instance.SelesaiBattle();
        }
    }

    void KalahBattle()
    {
        Debug.Log("HP Player Habis!");
        battlePanelUI.SetActive(false); 
        StartCoroutine(FadeManager.instance.StartFadeInOut());
    }

    public void PersiapkanBattle(int jumlahKosakata)
    {
        // LOG UNTUK PENGECEKAN AWAL (LIHAT DI CONSOLE UNITY)
        Debug.Log("[Battle] Menerima kiriman parameter jumlahKosakata dari StageManager: " + jumlahKosakata);
        Debug.Log("[Battle] Status isFinalBoss saat ini adalah: " + isFinalBoss);

        if (isFinalBoss)
        {
            if (InventoryManager.instance != null)
            {
                // Hitung total kata yang sudah terbuka (yang terjemahannya bukan "???")
                int totalKoleksi = InventoryManager.instance.kamusTobelo.Count(x => x.Value.kataTobelo != "???");
                
                Debug.Log("[Battle] Total kosakata yang berhasil ditemukan di Inventory: " + totalKoleksi);

                // Fail-safe jika ternyata kamus kosong, beri 3 HP agar tidak langsung mati
                if (totalKoleksi <= 0) 
                {
                    Debug.LogWarning("[Battle] Kamus global terdeteksi kosong atau belum ada yang terbuka! Memberikan HP default 3.");
                    totalKoleksi = 3; 
                }

                currentHPPlayer = totalKoleksi;
                currentHPMonster = totalKoleksi;
            }
            else
            {
                Debug.LogError("[Battle] InventoryManager.instance TIDAK DITEMUKAN di scene ini!");
                currentHPPlayer = 3; // Nilai cadangan jika InventoryManager hilang
                currentHPMonster = 3;
            }
        }
        else
        {
            // Mode monster biasa
            currentHPPlayer = jumlahKosakata;
            currentHPMonster = jumlahKosakata;

            if (currentHPPlayer <= 0)
            {
                Debug.LogWarning("[Battle] HP Monster Biasa diatur 0! Memberikan HP default 1 agar tidak langsung kalah.");
                currentHPPlayer = 1;
                currentHPMonster = 1;
            }
        }

        Debug.Log("[Battle] HP Berhasil Diatur -> Player HP: " + currentHPPlayer + " | Monster HP: " + currentHPMonster);

        // Isi antrean soal
        IsiUlangAntreanSoal();

        // Pastikan UI di-update SETELAH variabel HP di atas terisi nilai baru
        UpdateHPUI();
    }

    public void UpdateHPUI()
    {
        // Pengaman: Pastikan komponen teks UI tidak kosong (null)
        if (playerHPText != null && monsterHPText != null)
        {
            playerHPText.text = "x" + currentHPPlayer.ToString();
            monsterHPText.text = "x" + currentHPMonster.ToString();
            Debug.Log("[Battle UI] Teks HP Berhasil Diperbarui di Layar: Player=" + currentHPPlayer + ", Monster=" + currentHPMonster);
        }
        else
        {
            Debug.LogError("[Battle UI] Komponen Teks 'playerHPText' atau 'monsterHPText' belum di-drag ke slot Inspector BattleSystem!");
        }
    }

    // Fungsi bantu untuk mengisi ulang antrean kuis jika habis
    void IsiUlangAntreanSoal()
    {
        if (isFinalBoss)
        {
            antreanSoalFinalBoss.Clear();
            foreach (var item in InventoryManager.instance.kamusTobelo)
            {
                // Hanya masukkan kata yang sudah berhasil ditemukan player ke dalam ujian
                if (item.Value.kataTobelo != "???")
                {
                    antreanSoalFinalBoss.Add(item.Key); // item.Key adalah nama Indonesia
                }
            }
        }
        else
        {
            antreanSoal.Clear();
            foreach (GameObject obj in StageManager.instance.listItemTerambil)
            {
                if (obj != null && obj.GetComponent<ItemObjek>() != null)
                {
                    antreanSoal.Add(obj);
                }
            }
        }
    }

    // Coroutine untuk mengatur jeda waktu animasi layar hitam, teks tamat, dan pindah menu
    private System.Collections.IEnumerator SekuensGameTamat()
    {

        // JELAS & SINKRON: Tunggu di sini dulu agar monster selesai guncang/hancur
        yield return new WaitForSeconds(jedaSebelumTamatMuncul);

        // Ambil komponen CanvasGroup dari panel tamat
        CanvasGroup canvasGroupTamat = panelTamatUI.GetComponent<CanvasGroup>();
        if (canvasGroupTamat != null)
        {
            canvasGroupTamat.alpha = 0f; // Pastikan mulai dari transparan total
        }
        
        // Aktifkan objek panel hitamnya
        panelTamatUI.SetActive(true);

        // PROSES MEMUDAR (FADE IN BLACK): Layar perlahan menggelap pekat secara halus
        float counter = 0f;
        while (counter < kecepatanMunculTamat)
        {
            counter += Time.deltaTime;
            if (canvasGroupTamat != null)
            {
                // Menaikkan kepekatan dari 0 (transparan) ke 1 (hitam pekat + tulisan)
                canvasGroupTamat.alpha = Mathf.Lerp(0f, 1f, counter / kecepatanMunculTamat);
            }
            yield return null; 
        }

        if (canvasGroupTamat != null) canvasGroupTamat.alpha = 1f; // Pastikan hitam sempurna

        // Tahan layar TAMAT selama 4 detik agar pemain bisa membaca
        yield return new WaitForSeconds(4f);

        // Pindah ke Main Menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}