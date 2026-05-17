using UnityEngine;

public class DropManager : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private GameObject powerUpPrefab; // Yerdeki mavi küp
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.2f; // %20 þans

    private void OnEnable()
    {
        // OBSERVER PATTERN: Herhangi bir düþman öldüðünde haberdar ol
        EnemyController.OnEnemyDiedAtPosition += TryDropItem;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyDiedAtPosition -= TryDropItem;
    }

    private void TryDropItem(Vector3 spawnPosition)
    {
        // Rastgele þans kontrolü
        if (Random.value <= dropChance)
        {
            // PowerUp'ý düþmanýn öldüðü yerde oluþtur
            // Yerden biraz yukarýda doðmasý için Vector3.up ekleyebilirsin
            Instantiate(powerUpPrefab, spawnPosition + Vector3.up * 0.5f, Quaternion.identity);
            Debug.Log("<color=yellow>Þanslý günün! PowerUp düþtü.</color>");
        }
    }
}