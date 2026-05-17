using System;

public interface IDamageable
{
    // Objenin o anki can deðeri
    float CurrentHealth { get; }

    // Objenin sahip olabileceði maksimum can deðeri
    float MaxHealth { get; }

    // Objeye hasar vermek için çaðrýlan metot
    void TakeDamage(float amount);

    // Can yüzdesi deðiþtiðinde (CurrentHealth / MaxHealth) tetiklenir.
    // Observer pattern: Renk deðiþtirme (ColorFeedback) gibi sistemler bu olayý dinler.
    event Action<float> OnHealthPercentChanged;

    // Objenin caný 0 veya altýna düþtüðünde tetiklenir.
    // Observer pattern: Ölüm animasyonlarý, oyun bitiþi veya havuz (pool) iadeleri için dinlenir.
    event Action OnDied;
}
