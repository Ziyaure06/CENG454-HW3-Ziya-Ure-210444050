using UnityEngine;

public class RapidFireDecorator : WeaponDecorator
{
    private const float SpeedMultiplier = 3.0f;
    private float _nextFireTime = 0f;

    public RapidFireDecorator(IWeapon weaponToDecorate) : base(weaponToDecorate) { }

    // Decorated FireRate: temel silah hizini carparak seri ates saglar
    public override float FireRate => _decoratedWeapon.FireRate * SpeedMultiplier;

    public override void Fire(Transform firePoint, Vector3 direction)
    {
        // Decorator kendi ates hizi kontrolunu yapar
        if (Time.time < _nextFireTime) return;

        _decoratedWeapon.Fire(firePoint, direction);

        _nextFireTime = Time.time + (1f / FireRate);
    }
}