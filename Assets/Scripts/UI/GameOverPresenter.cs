// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Muestra panel de derrota cuando el jugador se queda sin vidas.
using UnityEngine;

public class GameOverPresenter : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private GameObject gameOverPanel;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.PlayerDied += OnPlayerDied;
        }
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.PlayerDied -= OnPlayerDied;
        }
    }

    private void OnPlayerDied()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
