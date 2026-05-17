using System;
using UnityEngine;

public class WallFoundation : MonoBehaviour, IInteractable, IDamageable
{
    [Header("Wall Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float repairSpeed = 25f;
    [SerializeField] private float maxHeightScale = 3f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    public static event Action OnWallCompleted;

    private bool _isCompleted = false;
    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
        CurrentHealth = 0f; // Oyun baþýnda yýkýk (0 can) baþlar

        // Baþlangýçta en altta ve kýpkýrmýzý olmasý için tetikliyoruz
        UpdateWallScaleAndColor();
    }

    public void Interact(float deltaTime)
    {
        if (_isCompleted) return;

        CurrentHealth += repairSpeed * deltaTime;
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);

        UpdateWallScaleAndColor();

        if (CurrentHealth >= maxHealth && !_isCompleted)
        {
            _isCompleted = true;
            OnWallCompleted?.Invoke();
            Debug.Log("Bir duvar baþarýyla inþa edildi!");
        }
    }

    public void TakeDamage(float amount)
    {
        // Canýmýz zaten 0 ise daha fazla hasar alýp batamaz
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(0, CurrentHealth);

        UpdateWallScaleAndColor();

        if (CurrentHealth <= 0)
        {
            if (_isCompleted)
            {
                _isCompleted = false;
                OnDied?.Invoke(); // Duvarýn yýkýldýðýný sisteme haber ver
                Debug.Log("Bir duvar tamamen yýkýldý ve zemine battý!");
            }
        }
    }

    private void UpdateWallScaleAndColor()
    {
        float percent = maxHealth > 0 ? CurrentHealth / maxHealth : 0;

        // 1. Ölçeklendirme: Can azaldýkça Y ekseninde zemine doðru pürüzsüzce girer (0.1f yok olmasýný önler)
        float currentHeight = Mathf.Lerp(0.1f, maxHeightScale, percent);
        transform.localScale = new Vector3(_initialScale.x, currentHeight, _initialScale.z);

        // 2. Renk Feedback: Can yüzdesini Observer'a (ColorFeedback) gönderir
        // %0 iken Kýrmýzý, %100 iken Yeþil olmasýný saðlar.
        OnHealthPercentChanged?.Invoke(percent);
    }
}