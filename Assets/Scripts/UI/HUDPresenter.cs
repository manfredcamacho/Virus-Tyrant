// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Sincroniza HUD con vidas, estado evolutivo y progreso de evolucion.
using UnityEngine;
using TMPro;

public class HUDPresenter : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private PlayerEvolutionController playerEvolution;
    [SerializeField] private LevelFlowController levelFlow;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text killsProgressText;
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text scoreTotalText;
    [SerializeField] private bool enableDebugLogs = true;

    private int totalScore;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.LivesChanged += OnLivesChanged;
        }

        if (playerEvolution != null)
        {
            playerEvolution.KillsProgressChanged += OnKillsProgressChanged;
        }

        if (levelFlow != null)
        {
            levelFlow.CurrentLevelChanged += OnCurrentLevelChanged;
            levelFlow.RemainingEnemiesChanged += OnRemainingEnemiesChanged;
        }

        EnemyHealth.EnemyKilled += OnEnemyKilled;
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            OnLivesChanged(playerHealth.CurrentLives);
        }

        if (playerEvolution != null)
        {
            OnKillsProgressChanged(playerEvolution.CurrentKills, playerEvolution.KillsToNextEvolution);
        }

        if (levelFlow != null)
        {
            OnCurrentLevelChanged(1);
            OnRemainingEnemiesChanged(0);
        }

        totalScore = 0;
        RefreshScore();
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.LivesChanged -= OnLivesChanged;
        }

        if (playerEvolution != null)
        {
            playerEvolution.KillsProgressChanged -= OnKillsProgressChanged;
        }

        if (levelFlow != null)
        {
            levelFlow.CurrentLevelChanged -= OnCurrentLevelChanged;
            levelFlow.RemainingEnemiesChanged -= OnRemainingEnemiesChanged;
        }

        EnemyHealth.EnemyKilled -= OnEnemyKilled;
    }

    private void OnLivesChanged(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Vidas: " + lives;
        }
        Log("HUD vidas -> " + lives);
    }

    private void OnRemainingEnemiesChanged(int remainingEnemies)
    {
        if (stateText != null)
        {
            stateText.text = "Enemigos restantes: " + Mathf.Max(0, remainingEnemies);
        }
        Log("HUD restantes -> " + remainingEnemies);
    }

    private void OnKillsProgressChanged(int current, int required)
    {
        if (killsProgressText != null)
        {
            killsProgressText.text = "Evolucion: " + current + "/" + required;
        }
        Log("HUD progreso -> " + current + "/" + required);
    }

    private void OnCurrentLevelChanged(int level)
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = "Nivel " + level + " - 3";
        }
    }

    private void OnEnemyKilled()
    {
        totalScore++;
        RefreshScore();
    }

    private void RefreshScore()
    {
        if (scoreTotalText != null)
        {
            scoreTotalText.text = "Score: " + totalScore;
        }
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[HUDPresenter] " + message, this);
        }
    }
}
