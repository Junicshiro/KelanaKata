using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 moveInput;
    private float currentSpeed;
    private bool isRunningUI = false; // Status lari dari UI

    void Update()
    {
        // 1. Ambil Input (Keyboard tetap dibiarkan agar bisa testing di PC)
        float keyboardX = Input.GetAxisRaw("Horizontal");
        float keyboardY = Input.GetAxisRaw("Vertical");

        // Jika tidak ada input UI, gunakan keyboard. Jika ada UI, gunakan UI.
        if (moveInput == Vector2.zero)
        {
            moveInput.x = keyboardX;
            moveInput.y = keyboardY;
        }

        // 2. Cek status lari (Keyboard Shift atau Tombol UI Run)
        bool isRunning = (Input.GetKey(KeyCode.LeftShift) || isRunningUI) && moveInput != Vector2.zero;
        
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed = (moveInput != Vector2.zero) ? targetSpeed : 0f;

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput.normalized * currentSpeed * Time.fixedDeltaTime);
    }

    // --- FUNGSI BARU UNTUK UI ---

    // Dipanggil saat tombol arah ditekan
    // Fungsi untuk Input X (Kanan/Kiri)
    public void SetInputX(float x)
    {
        moveInput.x = x;
    }

    // Fungsi untuk Input Y (Atas/Bawah)
    public void SetInputY(float y)
    {
        moveInput.y = y;
    }

    // Dipanggil saat tombol lari ditekan/lepas
    public void SetRunning(bool run)
    {
        isRunningUI = run;
    }

    void UpdateAnimation()
    {
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
        animator.SetFloat("Speed", currentSpeed);
    }
}