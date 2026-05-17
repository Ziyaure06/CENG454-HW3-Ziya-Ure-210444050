using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 6f;

    [Header("Weapon Settings")]
    [SerializeField] private Projectile bulletPrefab; // Unity'den ata
    [SerializeField] private Transform firePoint;     // Oyuncunun önünde boþ obje
    [SerializeField] private Transform bulletParent;  // Hierarchy düzeni için boþ obje
    [SerializeField] private float baseFireRate = 2f; // Saniyede 2 mermi

    public float CurrentHealth { get; private set; } = 1f;
    public float MaxHealth { get; } = 1f;

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    // DECORATOR PATTERN: Soyut arayüzü tutuyoruz
    public IWeapon CurrentWeapon { get; private set; }

    private Camera _mainCamera;
    private Rigidbody _rb;
    private Vector3 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;

        _rb.isKinematic = false;
        _rb.freezeRotation = true;
    }

    private void Start()
    {
        // Oyun baþýnda temel silahý kuþan (Constructor injection)
        CurrentWeapon = new BaseWeapon(bulletPrefab, baseFireRate, bulletParent);
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(moveX, 0, moveZ).normalized;

        RotateTowardsMouse();

        if (Input.GetKey(KeyCode.E))
        {
            CheckForInteractables();
        }

        // SOL TIK -> Ateþ Etmek (Mouse Yönüne)
        if (Input.GetMouseButton(0)) // GetMouseButton (basýlý tutunca) veya GetMouseButtonDown (tek týk)
        {
            ShootTowardsMouse();
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + _moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateTowardsMouse()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        int groundLayerMask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayerMask))
        {
            Vector3 targetPosition = hitInfo.point;
            targetPosition.y = transform.position.y;

            Vector3 direction = targetPosition - transform.position;
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    private void ShootTowardsMouse()
    {
        // 1. Farenin zemindeki noktasýný bul (Ateþ yönünü hesaplamak için)
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        int groundLayerMask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayerMask))
        {
            Vector3 targetMousePoint = hitInfo.point;
            // Y eksenindeki sapmayý önle (Mermiler dümdüz gitmeli)
            targetMousePoint.y = firePoint.position.y;

            // 2. Silah ucundan (firePoint) fare noktasýna giden yön vektörünü hesapla
            Vector3 fireDirection = targetMousePoint - firePoint.position;

            // 3. Silahý ateþle (Sözleþme çaðrýsý)
            if (fireDirection != Vector3.zero)
            {
                CurrentWeapon.Fire(firePoint, fireDirection);
            }
        }
    }

    private void CheckForInteractables()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(Time.deltaTime);
                break;
            }
        }
    }

    
    public void SetWeapon(IWeapon newWeapon)
    {
        CurrentWeapon = newWeapon;
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