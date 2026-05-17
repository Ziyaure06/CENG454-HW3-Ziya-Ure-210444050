using UnityEngine;

public class BaseWeapon : IWeapon
{
    protected Projectile _projectilePrefab;
    protected Transform _bulletParent;
    protected float _baseFireRate; // Inspector veya Constructor'dan gelen temel hýz

    private GenericObjectPool<Projectile> _projectilePool;
    private float _nextFireTime = 0f;

    public BaseWeapon(Projectile prefab, float fireRate, Transform bulletParent = null)
    {
        _projectilePrefab = prefab;
        _bulletParent = bulletParent;
        _baseFireRate = fireRate;
        _projectilePool = new GenericObjectPool<Projectile>(_projectilePrefab, 20, 100, _bulletParent);
    }

    // IWeapon'dan gelen zorunluluk: Temel hýzý geri döndürür.
    public virtual float FireRate => _baseFireRate;

    public void Fire(Transform firePoint, Vector3 direction)
    {
        // ÖNEMLÝ: Burada alt çizgi olmayan 'FireRate' (Property) kullanýlmalý!
        if (Time.time < _nextFireTime) return;

        Projectile bullet = _projectilePool.Get();
        bullet.transform.position = firePoint.position;
        bullet.ReturnToPoolAction = (b) => _projectilePool.Release(b);
        bullet.Launch(direction);

        // Bekleme süresini artýk dinamik olan FireRate'e göre hesaplýyoruz.
        _nextFireTime = Time.time + (1f / FireRate);
    }
}