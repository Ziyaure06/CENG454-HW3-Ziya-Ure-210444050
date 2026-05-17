using UnityEngine;

public class BaseWeapon : IWeapon
{
   
    protected Projectile _projectilePrefab;
    protected Transform _bulletParent;
    protected float _fireRate; 

    private GenericObjectPool<Projectile> _projectilePool;
    private float _nextFireTime = 0f;

    
    public BaseWeapon(Projectile prefab, float fireRate, Transform bulletParent = null)
    {
        _projectilePrefab = prefab;
        _bulletParent = bulletParent;
        _fireRate = fireRate;

        _projectilePool = new GenericObjectPool<Projectile>(_projectilePrefab, 20, 100, _bulletParent);
    }

    
    public virtual float FireRate => _fireRate;

    
    public void Fire(Transform firePoint, Vector3 direction)
    {
        
        if (Time.time < _nextFireTime) return;

        
        Projectile bullet = _projectilePool.Get();

        bullet.transform.position = firePoint.position;

       
        bullet.ReturnToPoolAction = (b) => _projectilePool.Release(b);

       
        bullet.Launch(direction);

       
        _nextFireTime = Time.time + (1f / FireRate);

        Debug.Log($"Ateþ edildi! Yön: {direction}. Sýradaki atýþ: {1f / FireRate}sn sonra.");
    }
}