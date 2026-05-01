// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Muestra panel de derrota cuando el jugador se queda sin vidas.
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverPresenter : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverTitleText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private Button backToMainMenuButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameOverTitle = "GAME OVER";
    [SerializeField] private bool stopTimeOnGameOver = true;
    [SerializeField] private bool enableDebugLogs = true;

    private int score;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.PlayerDied += OnPlayerDied;
        }

        EnemyHealth.EnemyKilled += OnEnemyKilled;
    }

    private void Start()
    {
        score = 0;
        Time.timeScale = 1f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (gameOverTitleText != null)
        {
            gameOverTitleText.text = gameOverTitle;
        }
        RefreshFinalScoreText();
        if (backToMainMenuButton != null)
        {
            backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        }
        Log("Start -> panel oculto y timeScale=1.");
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.PlayerDied -= OnPlayerDied;
        }

        EnemyHealth.EnemyKilled -= OnEnemyKilled;

        if (backToMainMenuButton != null)
        {
            backToMainMenuButton.onClick.RemoveListener(BackToMainMenu);
        }
    }

    private void OnEnemyKilled()
    {
        score++;
        RefreshFinalScoreText();
        Log("Score actualizado: " + score);
    }

    private void OnPlayerDied()
    {
        ShowGameOver();
        Log("PlayerDied -> game over mostrado" + (stopTimeOnGameOver ? " y juego pausado." : "."));
    }

    public void TriggerGameOverFromExternal(string customTitle = null)
    {
        if (!string.IsNullOrWhiteSpace(customTitle) && gameOverTitleText != null)
        {
            gameOverTitleText.text = customTitle;
        }

        ShowGameOver();
        Log("TriggerGameOverFromExternal -> game over forzado externamente.");
    }

    private void ShowGameOver()
    {
        RefreshFinalScoreText();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (stopTimeOnGameOver)
        {
            Time.timeScale = 0f;
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Log("No se puede volver al menú: mainMenuSceneName vacío.");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void RefreshFinalScoreText()
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + score;
        }
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[GameOverPresenter] " + message, this);
        }
    }
}
