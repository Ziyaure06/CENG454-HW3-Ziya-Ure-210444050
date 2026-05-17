using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public EnemyController enemyPrefab;

    [Tooltip("Baþlangýçta kaç saniyede bir düþman doðacak?")]
    public float initialSpawnDelay = 2f;

    [Tooltip("Her doðan düþmandan sonra süre ne kadar kýsalacak?")]
    public float spawnSpeedIncrease = 0.05f;

    [Tooltip("Düþman doðma hýzý en fazla ne kadar düþebilir? (Limit)")]
    public float minSpawnDelay = 0.5f;

    private GenericObjectPool<EnemyController> _enemyPool;
    private float _currentSpawnDelay;
    private float _nextSpawnTime;

    void Start()
    {
        _enemyPool = new GenericObjectPool<EnemyController>(enemyPrefab);
        _currentSpawnDelay = initialSpawnDelay;
        _nextSpawnTime = Time.time + _currentSpawnDelay;
    }

    void Update()
    {
        // Zamanlý spawner (Zamanla hýzlanan yapý)
        if (Time.time >= _nextSpawnTime)
        {
            SpawnEnemy();

            // Süreyi kýsalt (Zorluðu artýr)
            _currentSpawnDelay = Mathf.Max(minSpawnDelay, _currentSpawnDelay - spawnSpeedIncrease);
            _nextSpawnTime = Time.time + _currentSpawnDelay;
        }
    }

    void SpawnEnemy()
    {
        EnemyController enemy = _enemyPool.Get();

        // Düþmaný Spawner etrafýnda rastgele hafif daðýnýk bir pozisyonda doður
        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        enemy.transform.position = transform.position + randomOffset;

        // Kamikaze olup havuza dönebilmesi için Action'ý baðla
        enemy.ReturnToPoolAction = (e) => _enemyPool.Release(e);
    }
}