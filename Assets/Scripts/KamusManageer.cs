using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Wajib untuk membaca List<>

public class KamusManager : MonoBehaviour
{
    public GameObject itemPrefab; // Isi dengan Prefab ItemKamus
    public Transform contentArea; // Isi dengan objek Content dari ScrollView
    public AudioSource audioSource; // Referensi AudioSource untuk putar suara

    private void Start()
    {
        // Begitu pindah scene dan kamusmanager aktif, langsung gambar ulang UI-nya
        UpdateKamus();
    }

    public void UpdateKamus()
    {
        if (contentArea == null) return;

        // Bersihkan daftar lama di UI agar tidak double
        foreach (Transform child in contentArea) {
            Destroy(child.gameObject);
        }

        if (InventoryManager.instance == null) return;

        // SEKARANG: Looping diambil langsung dari InventoryManager yang abadi antar scene!
        foreach (var itemKamus in InventoryManager.instance.kamusTobelo)
        {
            string namaIndonesia = itemKamus.Key;
            string namaTobelo = itemKamus.Value.kataTobelo;
            AudioClip suaraKata = itemKamus.Value.suaraTobelo;

            // PENGAMAN: Di buku kamus, kita hanya ingin menampilkan kata yang SUDAH ditemukan saja.
            // Kata yang masih bernilai "???" (belum ditemukan player) akan dilewati / disembunyikan.
            if (namaTobelo == "???") continue;

            // Buat baris baru di UI Scroll View
            GameObject newEntry = Instantiate(itemPrefab, contentArea);
            
            // Isi teks ke Prefab
            newEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = namaIndonesia;
            newEntry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = namaTobelo;

            // Setup tombol play suara menggunakan file audio yang disimpan permanen di RAM tadi
            Button playBtn = newEntry.GetComponentInChildren<Button>();
            if (playBtn != null && suaraKata != null)
            {
                playBtn.onClick.AddListener(() => PutarSuara(suaraKata));
            }
        }
    }

    void PutarSuara(AudioClip clip)
    {
        if (clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }
}