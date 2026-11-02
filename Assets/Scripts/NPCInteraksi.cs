using UnityEngine;

public class NPCInteraksi : MonoBehaviour
{
    [Header("Identitas NPC")]
    public string namaNPC;
    public enum ArahHadap { Depan, Belakang, Kiri, Kanan }
    public ArahHadap arahAwal;

    [Header("Data Kata (Tobelo)")]
    public string kataIndonesia;
    public string kataTobelo;
    public string idKosakata;

    [Header("Dialog")]
    [TextArea(3, 10)]
    public string[] barisDialog;
    public AudioClip suaraTobelo;

    private Animator anim;
    private bool playerDekat = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        SetArah(arahAwal);
    }

    // Fungsi untuk mengatur arah hadap sprite lewat Animator
    void SetArah(ArahHadap arah)
    {
        float x = 0, y = 0;
        switch (arah)
        {
            case ArahHadap.Depan:   y = -1; break;
            case ArahHadap.Belakang: y = 1; break;
            case ArahHadap.Kiri:     x = -1; break;
            case ArahHadap.Kanan:    x = 1; break;
        }
        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", y);
    }

    public void MulaiDialog()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            MenolehKePlayer(player.transform.position);
        }

        DialogueManager.instance.StartDialogue(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDekat = true;
            other.GetComponent<PlayerInteraction>().SetNPCDidekat(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDekat = false;
            other.GetComponent<PlayerInteraction>().SetNPCDidekat(null);
        }
    }

    // Fungsi untuk membuat NPC menoleh ke Player
    public void MenolehKePlayer(Vector3 posisiPlayer)
    {
        // Hitung arah dari NPC ke Player
        Vector3 arah = (posisiPlayer - transform.position).normalized;

        // Tentukan arah mana yang paling dominan (Horizontal atau Vertical)
        if (Mathf.Abs(arah.x) > Mathf.Abs(arah.y))
        {
            // Menoleh Kiri atau Kanan
            float x = (arah.x > 0) ? 1 : -1;
            anim.SetFloat("Horizontal", x);
            anim.SetFloat("Vertical", 0);
        }
        else
        {
            // Menoleh Atas atau Bawah
            float y = (arah.y > 0) ? 1 : -1;
            anim.SetFloat("Horizontal", 0);
            anim.SetFloat("Vertical", y);
        }
    }
}