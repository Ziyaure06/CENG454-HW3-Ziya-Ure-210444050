using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamageable, IPoolable
{
    // --- STATÝK EVENT (DropManager için) ---
    public static event Action<Vector3> OnEnemyDiedAtPosition;

    [Header("Health Settings")]
    public float MaxHealth { get; private set; } = 1f;
    public float CurrentHealth { get; private set; }

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    public Action<EnemyController> ReturnToPoolAction;

    private NavMeshAgent _agent;
    private IMovementStrategy _currentStrategy;
    private bool _isDead = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void OnSpawn()
    {
        _isDead = false;
        CurrentHealth = MaxHealth;

        // %70 Base, %30 Player stratejisi
        if (UnityEngine.Random.value <= 0.7f)
            _currentStrategy = new TargetBaseStrategy();
        else
            _currentStrategy = new HuntPlayerStrategy();
    }

    private void Update()
    {
        if (!_isDead && _currentStrategy != null && _agent.isActiveAndEnabled)
        {
            _currentStrategy.ExecuteMove(_agent, transform);
        }
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        CurrentHealth -= amount;
        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;

        // 1. OBSERVER: DropManager'a pozisyon bildir
        OnEnemyDiedAtPosition?.Invoke(transform.position);

        // 2. EVENT: Diðer sistemlere haber ver
        OnDied?.Invoke();

        // 3. POOL: Havuza geri dön
        ReturnToPoolAction?.Invoke(this);
    }

    public void OnDespawn()
    {
        // Temizlik kurallarý
        OnHealthPercentChanged = null;
        OnDied = null;
        _currentStrategy = null;
        if (_agent.isActiveAndEnabled && _agent.isOnNavMesh) _agent.ResetPath();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        IDamageable hitObject = other.GetComponent<IDamageable>();
        if (hitObject != null && other.gameObject.tag != "Enemy") // Kendi arkadaþlarýna vurma
        {
            hitObject.TakeDamage(10f);
            TakeDamage(MaxHealth); // Kamikaze
        }
    }
}