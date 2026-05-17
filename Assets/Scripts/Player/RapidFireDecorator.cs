using UnityEngine;

public class RapidFireDecorator : WeaponDecorator
{
    [SerializeField] private float fireRateMultiplier = 2.0f; // Hýzý 2 katýna çýkar

    public RapidFireDecorator(IWeapon weaponToDecorate) : base(weaponToDecorate)
    {
        Debug.Log("<color=cyan>SÝLAH GÜÇLENDÝ: Seri Ateþ Modu Aktif!</color>");
    }

    public override void Fire(Transform firePoint, Vector3 direction)
    {
       
        base.Fire(firePoint, direction);
    }
}