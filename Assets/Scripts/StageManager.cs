using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header("Settings Stage")]
    public int totalKosakataHadir; 
    private int jumlahDiambil = 0;

    [Header("References")]
    public GameObject monsterObjek; // Tarik prefab/objek monster ke sini
    //list objek fisikk
    public List<GameObject> listItemTerambil = new List<GameObject>();
    // list kosakata NPC
    public List<string> listKataNpcStage = new List<string>();
    public GameObject battleArena;
    public GameObject portal;
    public GameObject hpPlayer;
    public GameObject hpMonster;
    public GameObject ScrollView;

    
    public bool apakahStageTerakhir;

    [Header("Checkpoint Settings")]
public Transform startStageCheckpoint; // Tarik objek kosong (Empty Object) sebagai titik mulai stage

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 🌟 FIREWALL PENGAMAN MUTLAK:
        // Begitu stage baru dimuat (termasuk saat playtest langsung dari Stage 2),
        // paksa InventoryManager untuk mengosongkan keranjang bawaan dari stage lalu!
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.kataDiStageSekarang.Clear();
            Debug.Log("Keranjang sementara berhasil dibersihkan untuk Stage ini.");
        }
    }

    public void TambahKosakata(GameObject item)
    {
        jumlahDiambil++;
        listItemTerambil.Add(item);

        Debug.Log("Kosakata terkumpul: " + jumlahDiambil + "/" + totalKosakataHadir);
        // 🌟 TAMBAHKAN INI: Beritahu QuestManager kalau kosakata bertambah
        if (FindObjectOfType<QuestManager>() != null)
        {
            FindObjectOfType<QuestManager>().TambahKosakata();
        }

        // Cek apakah sudah habis
        if (jumlahDiambil >= totalKosakataHadir)
        {
            MunculkanMonster();
            MunculkkanBattleArena();
        }
    }


    // Tambahkan fungsi ini di StageManager.cs
    public void SembunyikanItemDiMap()
    {
        // foreach (GameObject item in listItemTerambil)
        // {
        //     if (item != null)
        //     {
        //         item.SetActive(false); // Sembunyikan dari map
        //     }
        // }
        ScrollView.SetActive(false);
        Debug.Log("Kosakata disita oleh monster!");
    }

    // Pastikan fungsi KembalikanKosakata tetap seperti ini
    public void KembalikanKosakata()
    {
        // foreach (GameObject item in listItemTerambil)
        // {
        //     if (item != null)
        //     {
        //         item.SetActive(true); // Munculkan kembali di map
        //     }
        // }
        // Hapus baris listItemTerambil.Clear() jika kamu ingin 
        // item tersebut tetap ada di list untuk kebutuhan Kamus.
        // Jika ingin reset progres stage, baru gunakan Clear().
        ScrollView.SetActive(true);
        jumlahDiambil = 0; 
    }

    // Di dalam StageManager.cs
    void MunculkanMonster()
    {
        monsterObjek.SetActive(true);
        
        // Beritahu BattleSystem untuk set HP berdasarkan jumlah item yang diambil
        BattleSystem.instance.PersiapkanBattle(listItemTerambil.Count);
    }

    public void MunculkkanBattleArena()
    {
        battleArena.SetActive(true);
    }

    public void MunculkanHP()
    {
        hpPlayer.SetActive(true);
        hpMonster.SetActive(true);
    }

    public void SelesaiBattle()
    {
        monsterObjek.SetActive(false);
        KembalikanKosakata(); // Fungsi yang sudah kita buat sebelumnya
        battleArena.SetActive(false);
        hpPlayer.SetActive(false);
        hpMonster.SetActive(false);
        portal.SetActive(true);
        // 🌟 TAMBAHKAN INI SAAT MENANG
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.AmankanKataMenangBattle();
        }

        // 🌟 TAMBAHKAN INI: Beritahu QuestManager kalau monster sudah kalah
        if (FindObjectOfType<QuestManager>() != null)
        {
            FindObjectOfType<QuestManager>().KalahkanMonster();
        }

        Debug.Log("Monster kalah, kosakata kembali muncul!");
    }

    // Tambahkan ini di dalam StageManager.cs
    public void SembunyikanBattleUI()
    {
        // battleArena.SetActive(false);
        hpPlayer.SetActive(false);
        hpMonster.SetActive(false);
        BattleSystem.instance.battlePanelUI.SetActive(false);
        
    }

    public void GagalBattle()
    {
        // 🌟 JIKA BUKAN STAGE TERAKHIR
        if (!apakahStageTerakhir)
        {
            // Matikan Monster dan UI Battle agar Map bersih kembali
            if(monsterObjek != null) monsterObjek.SetActive(false);
            if(battleArena != null) battleArena.SetActive(false);
            
            SembunyikanBattleUI(); 

            // Munculkan kembali kosakata fisik di map
            foreach (GameObject itemObj in listItemTerambil)
            {
                if (itemObj != null)
                {
                    ItemObjek scriptItem = itemObj.GetComponent<ItemObjek>();
                    if (scriptItem != null) scriptItem.MunculkanKembali();
                }
            }

            // Reset kata yang baru diambil di stage biasa ini
            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.ResetKataGagalBattle();
            }

            KamusManager km = FindObjectOfType<KamusManager>();
            if (km != null) km.UpdateKamus();

            // Reset hitungan progres
            listItemTerambil.Clear();
            jumlahDiambil = 0;
            
            // 🌟 TAMBAHKAN INI: Reset tampilan Quest UI ke fase awal karena player gagal battle
            if (FindObjectOfType<QuestManager>() != null)
            {
                FindObjectOfType<QuestManager>().ResetQuestKeAwal();
            }
            Debug.Log("Player gagal di stage biasa. Kosakata direset ke map.");
        }
  
        // 🌟 KHUSUS JIKA STAGE TERAKHIR (BOSS FINAL)
        else
        {
            // Monster dan Arena TETAP AKTIF (tidak di-set false)
            if(monsterObjek != null) monsterObjek.SetActive(true);
            if(battleArena != null) battleArena.SetActive(true);
            
            // UI Darah dimatikan sementara (untuk di-reset full lagi saat combat dimulai ulang)
            SembunyikanBattleUI(); 

            // PENTING: Kosakata TIDAK DIRESET karena player tidak perlu mulung dari awal lagi
            Debug.Log("Player gagal di STAGE TERAKHIR! Monster dan Arena tetap aktif menunggu.");
        }

        // LOGIKA RESPOND/TELEPORT (Berlaku untuk semua stage agar player hidup lagi)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && startStageCheckpoint != null)
        {
            player.transform.position = startStageCheckpoint.position;
            
            if(player.GetComponent<Rigidbody2D>() != null)
            {
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
        }

        ScrollView.SetActive(true);
    }
}