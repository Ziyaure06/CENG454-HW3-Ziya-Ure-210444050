using UnityEngine;

public class GameStateTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CoreBase coreBase;
    [SerializeField] private PlayerController player;

    private int _completedWallsCount = 0;

    private void OnEnable()
    {
        // OBSERVER PATTERN: Tüm hayati olaylarý dinlemeye baþlýyoruz
        if (coreBase != null) coreBase.OnDied += HandleGameOver;
        if (player != null) player.OnDied += HandleGameOver;

        // Statik event aboneliði
        WallFoundation.OnWallCompleted += HandleWallCompleted;
    }

    private void OnDisable()
    {
        // BUG ÖNLEME: Abonelik iptalleri
        if (coreBase != null) coreBase.OnDied -= HandleGameOver;
        if (player != null) player.OnDied -= HandleGameOver;

        WallFoundation.OnWallCompleted -= HandleWallCompleted;
    }

    private void HandleWallCompleted()
    {
        _completedWallsCount++;
        Debug.Log($"Duvar Tamamlandý: {_completedWallsCount}/4");

        if (_completedWallsCount >= 4)
        {
            Debug.Log(" VICTORY! 4 Duvar da tamamlandý. Dünya kurtuldu!");
            Time.timeScale = 0f; // Oyunu dondur
        }
    }

    private void HandleGameOver()
    {
        Debug.Log(" GAME OVER! Üs yýkýldý veya Oyuncu öldü. ");
        Time.timeScale = 0f; // Oyunu dondur
    }
}