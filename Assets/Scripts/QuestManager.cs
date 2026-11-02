using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    // Definisikan urutan fase quest game kamu
    public enum QuestPhase { KumpulKosakata, KalahMonster, LanjutStage }
    public QuestPhase faseSekarang = QuestPhase.KumpulKosakata;

    [Header("Komponen UI")]
    public TextMeshProUGUI textQuestDeskripsi;
    public TextMeshProUGUI textCounterAngka;
    public Slider sliderProgressBar;

    [Header("Data Progress Game")]
    public int targetKosakata = 10;
    private int kosakataTerkumpul = 0;
    
    private int targetMonster = 1;
    private int monsterKalah = 0;

    // Variabel untuk animasi bar halus
    private float targetSliderValue = 0f;
    public float kecepatanBar = 5f;

    void Start()
    {
        UpdateQuestUI();
    }

    void Update()
    {
        // Membuat pergerakan slider bar menjadi halus mengalir (Smooth Lerp)
        sliderProgressBar.value = Mathf.Lerp(sliderProgressBar.value, targetSliderValue, Time.deltaTime * kecepatanBar);
    }

    // Fungsi yang dipanggil setiap kali ada progres (misal saat player menyentuh objek/NPC kosakata)
    public void TambahKosakata()
    {
        if (faseSekarang == QuestPhase.KumpulKosakata)
        {
            kosakataTerkumpul++;
            
            // Cek apakah quest 1 selesai
            if (kosakataTerkumpul >= targetKosakata)
            {
                kosakataTerkumpul = targetKosakata;
                GantiFaseQuest(QuestPhase.KalahMonster);
            }
            else
            {
                UpdateQuestUI();
            }
        }
    }

    // Fungsi yang dipanggil saat monster kalah di kuis
    public void KalahkanMonster()
    {
        if (faseSekarang == QuestPhase.KalahMonster)
        {
            monsterKalah++;
            if (monsterKalah >= targetMonster)
            {
                GantiFaseQuest(QuestPhase.LanjutStage);
            }
        }
    }

    // Fungsi untuk mengubah teks dan bar ketika pindah tugas berikutnya
    void GantiFaseQuest(QuestPhase faseBaru)
    {
        faseSekarang = faseBaru;
        UpdateQuestUI();
        
        // Efek suara atau partikel quest selesai bisa ditaruh di sini
        Debug.Log("Quest Diperbarui ke: " + faseBaru.ToString());
    }

    // Fungsi utama pengatur tampilan teks dan target bar di layar
    void UpdateQuestUI()
    {
        switch (faseSekarang)
        {
            case QuestPhase.KumpulKosakata:
                textQuestDeskripsi.text = "Kumpulkan Kosakata";
                textCounterAngka.text = kosakataTerkumpul + "/" + targetKosakata;
                targetSliderValue = (float)kosakataTerkumpul / targetKosakata;
                break;

            case QuestPhase.KalahMonster:
                textQuestDeskripsi.text = "Kalahkan Monster!";
                textCounterAngka.text = monsterKalah + "/" + targetMonster;
                targetSliderValue = (float)monsterKalah / targetMonster;
                break;

            case QuestPhase.LanjutStage:
                textQuestDeskripsi.text = "Ke Stage Selanjutnya";
                textCounterAngka.text = ""; // Tidak butuh angka untuk quest berjalan
                targetSliderValue = 1f; // Langsung penuhkan bar
                break;
        }
    }

    // 🌟 TAMBAHKAN FUNGSI BARU INI di dalam QuestManager.cs
    public void ResetQuestKeAwal()
    {
        faseSekarang = QuestPhase.KumpulKosakata;
        kosakataTerkumpul = 0; // Reset hitungan internal quest menjadi nol kembali
        monsterKalah = 0;
        UpdateQuestUI(); // Paksa UI untuk memperbarui teks dan bar saat itu juga
    }
}