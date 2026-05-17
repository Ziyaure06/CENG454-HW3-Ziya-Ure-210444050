using UnityEngine;


public abstract class WeaponDecorator : IWeapon
{
    protected IWeapon _decoratedWeapon;

    public WeaponDecorator(IWeapon weaponToDecorate)
    {
        _decoratedWeapon = weaponToDecorate;
    }

  
    public virtual void Fire(Transform firePoint, Vector3 direction)
    {
        _decoratedWeapon.Fire(firePoint, direction);
    }
}