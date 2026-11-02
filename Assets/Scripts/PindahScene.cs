using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalScene : MonoBehaviour
{
    // Tambahkan variabel static ini
    public static bool sedangPindahDariDalam = false;

    [Header("Pengaturan Scene")]
    public string namaSceneTujuan;
    public bool butuhKlik = true; // Jika dicentang, harus tekan E. Jika tidak, langsung pindah saat sentuh.

    [Header("Opsional: Animasi")]
    public Animator anim; // Kosongkan jika pintu tidak kelihatan

    private bool playerDekat = false;

    void Update()
    {
        if (playerDekat)
        {
            // if (butuhKlik && Input.GetKeyDown(KeyCode.E))
            // {
            //     MulaiPindah();
            // }
            if (!butuhKlik)
            {
                MulaiPindah();
            }
        }
    }

    void MulaiPindah()
    {
        // Jika ada animator (pintu luar), mainkan animasi dulu
        // if (anim != null)
        // {
        //     anim.SetBool("isOpen", true);
        //     Invoke("EksekusiPindah", 0.5f); // Jeda biar animasi kelihatan
        // }
        // if (anim = null)
        // {
            EksekusiPindah();
        // }
    }

    void EksekusiPindah()
    {
        sedangPindahDariDalam = true; // Set jadi true tepat sebelum pindah scene
        SceneManager.LoadScene(namaSceneTujuan);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerDekat = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerDekat = false;
    }
}