using UnityEngine;

public class MekanismePintu : MonoBehaviour
{
    public Animator anim;
    public BoxCollider2D physicalCollider; // Collider yang menghalangi jalan
    private bool isOpen = false;
    private bool playerDekat = false;

    // void Update()
    // {
    //     // Tekan E (atau tombol lain) saat berada di dekat pintu
    //     if (playerDekat && Input.GetKeyDown(KeyCode.E))
    //     {
    //         TogglePintu();
    //     }
    // }

    public void InteraksiPintuDalam()
    {
        isOpen = !isOpen;
        anim.SetBool("isOpen", isOpen);
        physicalCollider.enabled = !isOpen;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
            collision.GetComponent<PlayerInteraction>().SetPintuDalamDidekat(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
            collision.GetComponent<PlayerInteraction>().SetPintuDalamDidekat(null);
    }
}