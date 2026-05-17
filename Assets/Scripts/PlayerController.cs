using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    public float CurrentHealth { get; private set; } = 1f; // Tek caný var
    public float MaxHealth { get; } = 1f;

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    private Camera _mainCamera;
    private Rigidbody _rb;
    private Vector3 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;

        // Rigidbody ayarlarý
        _rb.isKinematic = false;
        _rb.freezeRotation = true; // Fizik motorunun karakteri devirmesini engelle
    }

    private void Update()
    {
        // 1. WASD Girdileri
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(moveX, 0, moveZ).normalized;

        // 2. Fare Yönüne Bakýþ (3rd Person Raycast)
        RotateTowardsMouse();

        // 3. Etkileþim Mekaniði ('E' Tuþu)
        if (Input.GetKey(KeyCode.E))
        {
            CheckForInteractables();
        }
    }

    private void FixedUpdate()
    {
        // Fizik tabanlý pürüzsüz hareket
        _rb.MovePosition(transform.position + _moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateTowardsMouse()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            Vector3 targetPosition = hitInfo.point;
            targetPosition.y = transform.position.y; // Y ekseninde eðilmeyi önle

            Vector3 direction = targetPosition - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    private void CheckForInteractables()
    {
        // Oyuncunun önündeki/etrafýndaki etkileþime geçilebilir objeleri tarar
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f); // 2 birim menzil
        foreach (var col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Sözleþme gereði deltaTime göndererek nesneyi tetikliyoruz (Tamir/Ýnþa)
                interactable.Interact(Time.deltaTime);
                break; // Ayný anda sadece tek bir objeyle etkileþime geç
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            OnDied?.Invoke();
        }
    }
}