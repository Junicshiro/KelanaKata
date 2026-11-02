using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private ItemObjek itemTersedia;
    private MekanismePintu pintuDalamTersedia;
    private MekanismePintuLuar pintuTransisiTersedia;
    
    // Variabel untuk menyimpan NPC yang sedang didekati
    private NPCInteraksi npcDidekat; 

    // Setters untuk Item dan Pintu
    public void SetItemDidekat(ItemObjek item) { itemTersedia = item; }
    public void SetPintuDalamDidekat(MekanismePintu pintu) { pintuDalamTersedia = pintu; }
    public void SetPintuTransisiDidekat(MekanismePintuLuar pintu) { pintuTransisiTersedia = pintu; }

    // Fungsi agar NPC bisa mendaftarkan dirinya saat player mendekat
    public void SetNPCDidekat(NPCInteraksi npc) 
    { 
        npcDidekat = npc; 
    }

    public void TombolInteraksiUI()
    {
        // 1. CEK DIALOG DULU (Prioritas Utama)
        // Jika panel sedang aktif, tombol ini berfungsi untuk lanjut teks
        if (DialogueManager.instance.dialoguePanel.activeSelf)
        {
            // Cek apakah saat ini sedang dalam mode Prolog
            if (DialogueManager.instance.isPrologMode)
            {
                // Jika sedang prolog, panggil antrean khusus prolog agar nama berganti
                DialogueManager.instance.TampilkanKalimatPrologBerikutnya();
            }
            else
            {
                // Jika gameplay biasa, panggil antrean normal bawaanmu
                DialogueManager.instance.DisplayNextSentence();
            }
            
            return; // WAJIB PAKAI RETURN agar kode di bawah tidak ikut jalan
        }

        // 2. JIKA TIDAK ADA DIALOG, CEK INTERAKSI BENDA (Gameplay Normal)
        if (itemTersedia != null)
        {
            ItemObjek item = itemTersedia;
            itemTersedia = null; // Mencegah klik ganda
            item.Ambil();
        }
        else if (pintuDalamTersedia != null)
        {
            pintuDalamTersedia.InteraksiPintuDalam();
        }
        else if (pintuTransisiTersedia != null)
        {
            pintuTransisiTersedia.InteraksiPintuTransisi();
        }
        else if (npcDidekat != null) 
        {
            npcDidekat.MulaiDialog();
        }
        else
        {
            Debug.Log("Tidak ada objek untuk diinteraksi.");
        }
    }
}