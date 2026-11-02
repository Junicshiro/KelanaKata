using UnityEngine;

public class ItemBlink : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Mulai dalam keadaan mati untuk optimasi
        animator.enabled = false;
    }

    void OnBecameVisible()
    {
        animator.enabled = true;
        animator.SetBool("isBlinking", true);
    }

    void OnBecameInvisible()
    {
        animator.SetBool("isBlinking", false);
        animator.enabled = false;
    }
}