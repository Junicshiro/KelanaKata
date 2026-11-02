using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [System.Serializable]
    public class DataKosakata
    {
        public string kataTobelo;
        public AudioClip suaraTobelo;
    }

    public Dictionary<string, DataKosakata> kamusTobelo = new Dictionary<string, DataKosakata>();
    public string[] daftarKataAwal = { "Teko", "Pisau", "Rumah", "Pintu" };

    // 🌟 KODE BARU 1: Keranjang sementara untuk stage yang sedang dimainkan
    public List<string> kataDiStageSekarang = new List<string>(); 

    private void OnApplicationQuit()
    {
        // Otomatis bersihkan kamus saat aplikasi ditutup
        ResetKamusTotalKeAwal();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InisialisasiKamus();
        }
        else { Destroy(gameObject); }
    }

    void InisialisasiKamus()
    {
        foreach (string kata in daftarKataAwal)
        {
            DataKosakata dataBaru = new DataKosakata();
            dataBaru.kataTobelo = "???";
            dataBaru.suaraTobelo = null;
            kamusTobelo.Add(kata, dataBaru);
        }
    }

    public void TemukanTerjemahan(string namaIndo, string kataTobelo, AudioClip suara)
    {
        if (kamusTobelo.ContainsKey(namaIndo))
        {
            kamusTobelo[namaIndo].kataTobelo = kataTobelo;
            kamusTobelo[namaIndo].suaraTobelo = suara;
        }
        else
        {
            DataKosakata dataBaru = new DataKosakata();
            dataBaru.kataTobelo = kataTobelo;
            dataBaru.suaraTobelo = suara;
            kamusTobelo.Add(namaIndo, dataBaru);
        }

        // 🌟 KODE BARU 2: Otomatis catat kata ini ke keranjang sementara
        if (!kataDiStageSekarang.Contains(namaIndo))
        {
            kataDiStageSekarang.Add(namaIndo);
        }
    }

    // 🌟 KODE BARU 3: Fungsi jika KALAH (Reset isi keranjang sementara)
    public void ResetKataGagalBattle()
    {
        foreach (string kata in kataDiStageSekarang)
        {
            if (kamusTobelo.ContainsKey(kata))
            {
                kamusTobelo[kata].kataTobelo = "???";
                kamusTobelo[kata].suaraTobelo = null;
            }
        }
        kataDiStageSekarang.Clear(); // Buang isi keranjang
        Debug.Log("Kosakata gagal diselamatkan dan direset!");
    }

    // 🌟 KODE BARU 4: Fungsi jika MENANG (Amankan kata jadi permanen)
    public void AmankanKataMenangBattle()
    {
        kataDiStageSekarang.Clear(); // Kosongkan keranjang tanpa mereset dictionary-nya
        Debug.Log("Stage Clear! Kosakata permanen diamankan.");
    }

    // 🌟 KODE BARU: Reset total kamus kembali menjadi "???" saat keluar ke Main Menu
    public void ResetKamusTotalKeAwal()
    {
        if (InventoryManager.instance != null)
        {
            // 1. Matikan referensi static instance-nya agar tidak error saat memuat yang baru
            GameObject objekInventory = InventoryManager.instance.gameObject;
            InventoryManager.instance = null; 

            // 2. Hancurkan objek fisiknya dari memori game secara instan
            Destroy(objekInventory);
            
            Debug.Log("[Inventory System] Objek abadi berhasil dihancurkan. Memori bersih!");
        }

        
    }
}