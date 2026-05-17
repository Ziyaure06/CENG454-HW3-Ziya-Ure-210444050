using UnityEngine;

public class RapidFireDecorator : WeaponDecorator
{
    private const float SpeedMultiplier = 3.0f;

    public RapidFireDecorator(IWeapon weaponToDecorate) : base(weaponToDecorate) { }

    // ARTIK HATA VERMEYECEK: Çünkü üst sýnýfta (WeaponDecorator) virtual bir FireRate var.
    public override float FireRate => _decoratedWeapon.FireRate * SpeedMultiplier;

    public override void Fire(Transform firePoint, Vector3 direction)
    {
        base.Fire(firePoint, direction);
    }
}