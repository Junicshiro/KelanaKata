using UnityEngine;
using System.Collections;

public class FeedbackEfek : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color warnaAsli;
    private Vector3 posisiAsliLokal; // Sekarang ini akan mencatat posisi ter-update

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            warnaAsli = spriteRenderer.color;
        }
        
        // Hapus pengambilan posisiAsliLokal dari sini agar tidak mengunci koordinat awal game
    }

    public void MainkanEfek(float durasi, float kekuatanGuncang)
    {
        StopAllCoroutines();
        
        if (spriteRenderer != null) spriteRenderer.color = warnaAsli;

        // PERBAIKAN UTAMA: Catat posisi lokal sekarang (saat kuis salah/benar dipicu)
        posisiAsliLokal = transform.localPosition;

        StartCoroutine(CoroutineMemerah(durasi));
        StartCoroutine(CoroutineGuncang(durasi, kekuatanGuncang));
    }

    private IEnumerator CoroutineMemerah(float durasi)
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(durasi * 0.5f);

        float timer = 0f;
        float durasiSisa = durasi * 0.5f;
        while (timer < durasiSisa)
        {
            timer += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(Color.red, warnaAsli, timer / durasiSisa);
            yield return null;
        }

        spriteRenderer.color = warnaAsli;
    }

    private IEnumerator CoroutineGuncang(float durasi, float kekuatan)
    {
        float timer = 0f;

        while (timer < durasi)
        {
            timer += Time.deltaTime;

            // Hitung guncangan berdasarkan posisi asli yang baru saja dicatat
            float acakX = Random.Range(-1f, 1f) * kekuatan;
            float acakY = Random.Range(-1f, 1f) * kekuatan;

            transform.localPosition = new Vector3(posisiAsliLokal.x + acakX, posisiAsliLokal.y + acakY, posisiAsliLokal.z);

            yield return null;
        }

        // Kembalikan ke posisi tepat sebelum dia diguncang tadi
        transform.localPosition = posisiAsliLokal;
    }
}