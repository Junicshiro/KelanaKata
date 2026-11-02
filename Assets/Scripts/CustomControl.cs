using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; // Tambahkan ini untuk Coroutine

public class CustomControl : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    public string namaTombol;
    // Variabel statis agar bisa diakses dengan mudah oleh MenuManager
    public static bool bolehEditLayout = false; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Jangan langsung Load di Start, beri jeda sedikit agar Canvas siap
        StartCoroutine(JedaLoad());
    }

    IEnumerator JedaLoad()
    {
        // Tunggu sampai akhir frame agar semua perhitungan UI selesai
        yield return new WaitForEndOfFrame();
        LoadPosisi();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // JIKA TIDAK DALAM MODE EDIT, JANGAN IZINKAN UI BERGESER!
        if (!bolehEditLayout) return;

        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void UbahUkuran(float nilai)
    {
        if (nilai <= 0) nilai = 0.1f;
        rectTransform.localScale = new Vector3(nilai, nilai, 1);
    }

    public void SimpanPosisi()
    {
        PlayerPrefs.SetFloat(namaTombol + "_X", rectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat(namaTombol + "_Y", rectTransform.anchoredPosition.y);
        PlayerPrefs.SetFloat(namaTombol + "_Size", rectTransform.localScale.x);
        PlayerPrefs.Save();
        Debug.Log("Tersimpan: " + namaTombol + " di " + rectTransform.anchoredPosition);
    }

    public void LoadPosisi()
    {
        if (PlayerPrefs.HasKey(namaTombol + "_X"))
        {
            float x = PlayerPrefs.GetFloat(namaTombol + "_X");
            float y = PlayerPrefs.GetFloat(namaTombol + "_Y");
            float s = PlayerPrefs.GetFloat(namaTombol + "_Size");

            rectTransform.anchoredPosition = new Vector2(x, y);
            rectTransform.localScale = new Vector3(s, s, 1);
            Debug.Log("Berhasil Load: " + namaTombol + " di Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}