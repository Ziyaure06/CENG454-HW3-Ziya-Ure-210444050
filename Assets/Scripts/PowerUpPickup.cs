using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PowerUpPickup : MonoBehaviour
{
    private void Awake()
    {
        // Çarpýþma algýlamak için Trigger olmalý
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Sadece Player çarpýnca çalýþsýn
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // OBSERVER PATTERN / DECORATOR ENTEGRASYONU:
            // Oyuncunun mevcut silah referansýný alýp güçlendiriyoruz
            IWeapon currentWeapon = player.CurrentWeapon;

            // Silahý RapidFireDecorator ile sar (wrap) ve oyuncuya geri ver
            IWeapon upgradedWeapon = new RapidFireDecorator(currentWeapon);
            player.SetWeapon(upgradedWeapon);

            // Güçlendirme alýndýktan sonra kendini yok et
            Debug.Log("PowerUp alýndý!");
            Destroy(gameObject);
        }
    }
}