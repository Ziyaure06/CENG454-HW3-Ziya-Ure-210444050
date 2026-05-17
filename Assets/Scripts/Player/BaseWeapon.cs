using UnityEngine;

public class BaseWeapon : IWeapon
{
    protected Projectile _projectilePrefab;
    protected Transform _bulletParent;
    protected float _baseFireRate;

    private GenericObjectPool<Projectile> _projectilePool;
    private float _nextFireTime = 0f;

    public BaseWeapon(Projectile prefab, float fireRate, Transform bulletParent = null)
    {
        _projectilePrefab = prefab;
        _bulletParent = bulletParent;
        _baseFireRate = fireRate;
        _projectilePool = new GenericObjectPool<Projectile>(_projectilePrefab, 20, 100, _bulletParent);
    }

    public virtual float FireRate => _baseFireRate;

    public virtual void Fire(Transform firePoint, Vector3 direction)
    {
        if (Time.time < _nextFireTime) return;

        Projectile bullet = _projectilePool.Get();
        bullet.transform.position = firePoint.position;
        bullet.ReturnToPoolAction = (b) => _projectilePool.Release(b);
        bullet.Launch(direction);

        _nextFireTime = Time.time + (1f / FireRate);
    }
}