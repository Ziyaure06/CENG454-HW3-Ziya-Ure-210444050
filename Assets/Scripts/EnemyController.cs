using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamageable, IPoolable
{
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; } = 1f;

    public event Action<float> OnHealthPercentChanged;
    public event Action OnDied;

    public Action<EnemyController> ReturnToPoolAction;

    private NavMeshAgent _agent;
    private IMovementStrategy _currentStrategy;

    // BUG FIX: Çift ölüm korumasý için bayrak
    private bool _isDead = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void OnSpawn()
    {
        CurrentHealth = MaxHealth;
        _isDead = false; // Havuzdan yeni çýkarken diriltiyoruz!

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
        if (!_isDead && _currentStrategy != null && _agent.isActiveAndEnabled)
        {
            _currentStrategy.ExecuteMove(_agent, transform);
        }
    }

    public void TakeDamage(float amount)
    {
        // BUG FIX: Eðer zaten öldüysek, gelen ekstra hasarlarý ve havuz taleplerini yoksay!
        if (_isDead) return;

        CurrentHealth -= amount;
        OnHealthPercentChanged?.Invoke(CurrentHealth / MaxHealth);

        if (CurrentHealth <= 0)
        {
            _isDead = true; // Objeyi ölü olarak iþaretle ki bir daha tetiklenmesin
            OnDied?.Invoke();
            ReturnToPoolAction?.Invoke(this);
        }
    }

    public void OnDespawn()
    {
        OnHealthPercentChanged = null;
        OnDied = null;
        _currentStrategy = null;

        if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eðer zaten öldüysek çarpýþmayý yoksay
        if (_isDead) return;

        IDamageable hitObject = other.GetComponent<IDamageable>();
        if (hitObject != null)
        {
            hitObject.TakeDamage(10f);
            TakeDamage(CurrentHealth);
        }
    }
}