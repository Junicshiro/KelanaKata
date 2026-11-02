using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TMPLinkClickHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI totalText;

    void Awake()
    {
        totalText = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Cek apakah yang diklik adalah bagian dari teks link TMP
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(totalText, eventData.position, eventData.pressEventCamera);
        
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = totalText.textInfo.linkInfo[linkIndex];
            string idLink = linkInfo.GetLinkID(); // Mengambil ID dari <link="ID">

            // Cari data kosakata berdasarkan ID link tersebut melalui database NPC atau Item
            MintaDataKosakataKeNPC(idLink);
        }
    }

    void MintaDataKosakataKeNPC(string idKosakata)
    {
        // Logika ini akan menghubungkan ID kata yang diklik dengan data asli di NPC kamu.
        // Untuk sementara, kita bisa meminta data dari NPC aktif melalui DialogueManager.
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.PicuKartuDariDialog(idKosakata);
        }
    }
}