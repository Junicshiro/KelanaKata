using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Wajib ditambahkan

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject kamusPanel;
    public AudioSource uiSource; 
    public AudioClip soundOpen;   
    public AudioClip soundClick;  

    [Header("Kamus Scroll Settings")]
    public GameObject itemPrefab; // Tarik Prefab ItemKamus ke sini
    public Transform contentArea; // Tarik Viewport > Content ke sini

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        uiSource.PlayOneShot(soundOpen);
    }

    // public void Resume()
    // {
    //     pausePanel.SetActive(false);
    //     kamusPanel.SetActive(false);
    //     isPaused = false;
    //     uiSource.PlayOneShot(soundOpen);

    //     if (DialogueManager.instance != null && !DialogueManager.instance.dialoguePanel.activeSelf)
    //     {
    //         Time.timeScale = 1f;
    //         GameObject player = GameObject.FindGameObjectWithTag("Player");
    //         // Pastikan nama script Movement kamu benar (Movement atau PlayerMovement)
    //         if(player != null && player.GetComponent<Movement>() != null) 
    //             player.GetComponent<Movement>().enabled = true;
    //     }
    // }
    public void Resume()
    {
        pausePanel.SetActive(false);
        kamusPanel.SetActive(false);
        isPaused = false;

        // Mainkan suara buka menu
        uiSource.PlayOneShot(soundOpen);

        // CEK: Jika panel dialog TIDAK aktif, baru boleh jalankan waktu
        // Kita cek ke DialogueManager.instance.dialoguePanel
        if (DialogueManager.instance != null && !DialogueManager.instance.dialoguePanel.activeSelf)
        {
            Time.timeScale = 1f;
            
            // Aktifkan kembali movement player jika memang tidak ada dialog
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null) player.GetComponent<PlayerMovement>().enabled = true;
        }
        else
        {
            // Jika dialog masih ada, biarkan Time.timeScale tetap 0
            Debug.Log("Waktu tetap berhenti karena dialog masih aktif.");
        }
    }

    public void BukaKamus()
    {
        pausePanel.SetActive(false);
        kamusPanel.SetActive(true);
        uiSource.PlayOneShot(soundOpen);
        
        // Panggil fungsi baru untuk generate daftar kosakata
        GenerateDaftarKamus(); 
    }

    // FUNGSI BARU UNTUK SCROLL VIEW DINAMIS
    void GenerateDaftarKamus()
    {
        // 1. Bersihkan isi Content agar tidak menumpuk saat buka-tutup kamus
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // 2. Ambil data dari StageManager (listItemTerambil)
        // Kita pakai data dari StageManager karena di sana tersimpan object ItemObjek yang punya file Audio
        foreach (GameObject obj in StageManager.instance.listItemTerambil)
        {
            ItemObjek data = obj.GetComponent<ItemObjek>();

            // 3. Munculkan prefab ke dalam Content area
            GameObject newEntry = Instantiate(itemPrefab, contentArea);

            // 4. Isi teks (Asumsi hirarki prefab: anak 0 = Indonesia, anak 1 = Tobelo)
            // Sesuaikan GetChild(x) dengan urutan objek di prefab kamu
            newEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.kataIndonesia;
            newEntry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = data.kataTobelo;

            // 5. Setup Tombol Play Suara
            Button playBtn = newEntry.GetComponentInChildren<Button>();
            if (playBtn != null && data.suaraTobelo != null)
            {
                playBtn.onClick.AddListener(() => PutarSuara(data.suaraTobelo));
            }
        }
    }

    void PutarSuara(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    public void PlayClickSound()
    {
        uiSource.PlayOneShot(soundClick);
    }
    
    public void KembaliKePause()
    {
        kamusPanel.SetActive(false);
        pausePanel.SetActive(true);
        uiSource.PlayOneShot(soundOpen);
    }

    public void KeluarGame()
    {
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.ResetKamusTotalKeAwal();
            
            // OPSIONAL: Jika kamu ingin menghancurkan objek Inventory-nya sekalian agar bersih total
            // Destroy(InventoryManager.instance.gameObject); 
        }
        Time.timeScale = 1f; // Selalu reset timeScale sebelum pindah scene
        SceneManager.LoadScene("MainMenu");
    }
}