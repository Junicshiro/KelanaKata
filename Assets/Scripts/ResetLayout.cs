using UnityEngine;

public class ResetLayout : MonoBehaviour
{
    [System.Serializable]
    public struct DataLayoutAwal
    {
        public RectTransform targetUI; // Elemen UI yang ingin direset
        [HideInInspector] public Vector2 posisiAwal;
        [HideInInspector] public Vector2 ukuranAwal;
        [HideInInspector] public Vector3 skalaAwal;
    }

    [Header("Daftar UI yang Ingin Direset")]
    public DataLayoutAwal[] daftarUI;

    void Start()
    {
        // Catat posisi, ukuran, dan skala awal semua UI saat game baru dimulai
        for (int i = 0; i < daftarUI.Length; i++)
        {
            if (daftarUI[i].targetUI != null)
            {
                daftarUI[i].posisiAwal = daftarUI[i].targetUI.anchoredPosition;
                daftarUI[i].ukuranAwal = daftarUI[i].targetUI.sizeDelta;
                daftarUI[i].skalaAwal = daftarUI[i].targetUI.localScale;
            }
        }
    }

    // Fungsi ini yang akan dipanggil oleh Tombol Reset di UI (OnClick)
    public void ResetKePosisiSemula()
    {
        for (int i = 0; i < daftarUI.Length; i++)
        {
            if (daftarUI[i].targetUI != null)
            {
                // Kembalikan semua nilai ke kondisi awal
                daftarUI[i].targetUI.anchoredPosition = daftarUI[i].posisiAwal;
                daftarUI[i].targetUI.sizeDelta = daftarUI[i].ukuranAwal;
                daftarUI[i].targetUI.localScale = daftarUI[i].skalaAwal;
            }
        }
        
        Debug.Log("Layout berhasil direset ke posisi dan ukuran semula!");
    }
}