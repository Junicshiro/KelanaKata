using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PrologAutoTrigger : MonoBehaviour
{
    [System.Serializable]
    public class BarisDialogProlog
    {
        public string namaKarakter; 
        [TextArea(2, 5)]
        public string kalimat;      
    }

    [Header("Daftar Percakapan Prolog")]
    public List<BarisDialogProlog> daftarDialogProlog = new List<BarisDialogProlog>();

    [Header("Pengaturan Jeda")]
    //[ Lenz: "Durasi animasi fade di FadeManager adalah 1 detik, jadi beri jeda sedikit lebih lama" ]
    public float jedaSebelumDialog = 1.2f; 

    IEnumerator Start()
    {
        // Tunggu sampai animasi fade selesai (layar terang sempurna)
        yield return new WaitForSeconds(jedaSebelumDialog);
        
        MulaiDialogOtomatis();
    }

    void MulaiDialogOtomatis()
    {
        if (DialogueManager.instance != null && daftarDialogProlog.Count > 0)
        {
            DialogueManager.instance.isPrologMode = true;
            
            // Panggil fungsi dialog multi-karakter
            DialogueManager.instance.StartPrologMultiKarakter(daftarDialogProlog);
        }
    }
}