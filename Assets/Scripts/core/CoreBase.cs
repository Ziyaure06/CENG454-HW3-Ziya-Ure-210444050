using System;
using UnityEngine;

public class CoreBase : MonoBehaviour, IDamageable
{
    [Header("Core Settings")]
    [Tooltip("Core'un oyuna baþlarken sahip olacaðý maksimum can.")]
    [SerializeField] private float initialHealth = 100f;

    // IDamageable arayüzünden gelen zorunlu alanlar
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

    public void TakeDamage(float amount)
    {
        
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;

       
        CurrentHealth = Mathf.Max(0, CurrentHealth);

      
        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);

       
        if (CurrentHealth <= 0)
        {
            OnDied?.Invoke();
            Debug.Log("CORE DESTROYED! (Phase 5'te Game Over sistemine baðlanacak)");
        }
    }
}