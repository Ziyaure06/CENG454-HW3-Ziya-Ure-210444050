using UnityEngine;

public interface IWeapon
{
    // Silahýn ateþleme eylemini gerçekleþtirir.
    // firePoint: Merminin çýkýþ pozisyonu ve rotasyonu (Silahýn ucu).
    // direction: Merminin hedefe doðru gideceði yön vektörü.
    void Fire(Transform firePoint, Vector3 direction);
}