using UnityEngine;

public class UILayoutManager : MonoBehaviour
{
    // Seret Dpad, Tombol A, dan Tombol B ke slot ini di Inspector
    public CustomControl[] listTombol; 

    public void KlikSimpan()
    {
        foreach (CustomControl t in listTombol)
        {
            t.SimpanPosisi();
        }
        Debug.Log("Posisi UI Berhasil Disimpan!");
    }
}