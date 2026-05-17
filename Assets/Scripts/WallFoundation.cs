using System;
using UnityEngine;

public class WallFoundation : MonoBehaviour, IInteractable, IDamageable
{
    [Header("Wall Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float repairSpeed = 25f; // Saniyede kaç can tamir olacak?
    [SerializeField] private float maxHeightScale = 3f; // Duvar tamamlanýnca Y boyutu ne olacak?

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    // OBSERVER PATTERN: Oyun durumunu takip eden sýnýf (GameStateTracker) bunu dinleyecek
    public static event Action OnWallCompleted;

    private bool _isCompleted = false;
    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
        CurrentHealth = 0f; // Duvar yýkýk (0 can) baþlar

        // Ýlk baþta tamamen basýk/yerde görünsün
        UpdateWallScale();
        OnHealthPercentChanged?.Invoke(0f);
    }

    // IInteractable Sözleþmesi: Oyuncu 'E'ye bastýkça çaðrýlýr
    public void Interact(float deltaTime)
    {
        if (_isCompleted) return;

        CurrentHealth += repairSpeed * deltaTime;
        CurrentHealth = Mathf.Min(CurrentHealth, maxHealth);

        UpdateWallScale();
        OnHealthPercentChanged?.Invoke(CurrentHealth / maxHealth);

        if (CurrentHealth >= maxHealth && !_isCompleted)
        {
            _isCompleted = true;
            OnWallCompleted?.Invoke(); // Duvar bitti olayý fýrlatýlýr
            Debug.Log("Bir duvar baþarýyla inþa edildi!");
        }
    }

    private void UpdateWallScale()
    {
        // Can yüzdesine göre Y eksenindeki scale deðerini artýrýyoruz
        float percent = maxHealth > 0 ? CurrentHealth / maxHealth : 0;
        float currentHeight = Mathf.Lerp(0.1f, maxHeightScale, percent);
        transform.localScale = new Vector3(_initialScale.x, currentHeight, _initialScale.z);
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(0, CurrentHealth);

        UpdateWallScale();
        OnHealthPercentChanged?.Invoke(CurrentHealth / maxHealth);

        if (CurrentHealth <= 0 && _isCompleted)
        {
            _isCompleted = false;
            OnDied?.Invoke();
        }
    }
}