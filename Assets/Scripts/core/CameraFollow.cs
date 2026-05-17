using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target; // Takip edilecek hedef (Player)
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -5f); // Kameranýn mesafesi
    [SerializeField] private float smoothTime = 0.2f; // Kamera takip yumuþaklýðý

    private Vector3 _currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        // Kameranýn gitmesi gereken hedef pozisyonu hesapla
        Vector3 targetPosition = target.position + offset;

        // Kamerayý pürüzsüz bir þekilde o pozisyona taþý (Rotasyonu asla deðiþmez!)
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
    }
}