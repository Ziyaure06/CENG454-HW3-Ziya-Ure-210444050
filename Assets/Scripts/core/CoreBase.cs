using System;
using UnityEngine;

public class CoreBase : MonoBehaviour, IDamageable, IInteractable
{
    [Header("Core Settings")]
    [SerializeField] private float initialHealth = 100f;
    [SerializeField] private float repairSpeed = 20f; // Oyuncu E'ye bastýkça saniyede yenilenen can

    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    private void Start()
    {
        MaxHealth = initialHealth;
        CurrentHealth = MaxHealth;
        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);
    }

   
    public void Interact(float deltaTime)
    {
        if (CurrentHealth <= 0 || CurrentHealth >= MaxHealth) return;

        CurrentHealth += repairSpeed * deltaTime;
        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);

        
        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(0, CurrentHealth);

        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            OnDied?.Invoke();
        }
    }
}