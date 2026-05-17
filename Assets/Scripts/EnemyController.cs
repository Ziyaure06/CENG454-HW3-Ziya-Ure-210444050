using System;
using UnityEngine;
using UnityEngine.AI;

// Bu script eklendiðinde NavMeshAgent'ý otomatik olarak ekler
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamageable, IPoolable
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; } = 1f;

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    // Havuz sisteminin bu objeyi geri alabilmesi için Action
    public Action<EnemyController> ReturnToPoolAction;

    private NavMeshAgent _agent;
    private IMovementStrategy _currentStrategy;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void OnSpawn()
    {
        CurrentHealth = MaxHealth;

        // %70 ihtimalle Base'e, %30 ihtimalle Player'a git
        if (UnityEngine.Random.value <= 0.7f)
        {
            _currentStrategy = new TargetBaseStrategy();
            gameObject.name = "Enemy (Targeting Core)";
        }
        else
        {
            _currentStrategy = new HuntPlayerStrategy();
            gameObject.name = "Enemy (Hunting Player)";
        }
    }

    private void Update()
    {
        // Atanan stratejiyi her frame çalýþtýr
        if (_currentStrategy != null && _agent.isActiveAndEnabled)
        {
            _currentStrategy.ExecuteMove(_agent, transform);
        }
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            OnDied?.Invoke();
            ReturnToPoolAction?.Invoke(this); // Havuza geri yollanma talebi
        }
    }

    public void OnDespawn()
    {
        // BUG ÖNLEME KURALI (Debug #003): Tüm eventleri temizle!
        OnHealthPercentChanged = null;
        OnDied = null;
        _currentStrategy = null; // Stratejiyi temizle ki çöp toplayýcý (GC) silsin

        if (_agent.isOnNavMesh)
        {
            _agent.ResetPath(); // Hedefi sýfýrla
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Çarpýlan obje bir IDamageable ise ona hasar ver ve kendini yok et (Havuza dön)
        IDamageable hitObject = collision.gameObject.GetComponent<IDamageable>();
        if (hitObject != null)
        {
            hitObject.TakeDamage(10f); // Base'e veya Player'a 10 hasar vur
            TakeDamage(CurrentHealth); // Kamikaze mantýðý: Vurunca ölür
        }
    }
}
