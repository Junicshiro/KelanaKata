using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Tarik objek Player ke sini
    public float smoothing = 5f; // Semakin besar, semakin cepat kamera mengikuti

    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0, 0, -10); // Jarak aman kamera (Z harus negatif)

    void FixedUpdate()
    {
        if (target != null)
        {
            // Tentukan posisi tujuan (Posisi player + jarak offset)
            Vector3 targetPosition = target.position + offset;

            // Gerakkan kamera secara halus dari posisi sekarang ke posisi tujuan
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.fixedDeltaTime);
            
            // Terapkan ke kamera
            transform.position = smoothedPosition;
        }
    }
}