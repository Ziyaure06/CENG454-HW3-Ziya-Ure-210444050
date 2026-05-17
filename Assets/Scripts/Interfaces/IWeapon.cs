using UnityEngine;

public interface IWeapon
{
    // Hata buradaydý: Bu satýrýn mutlaka olmasý gerekiyor!
    float FireRate { get; }

    void Fire(Transform firePoint, Vector3 direction);
}