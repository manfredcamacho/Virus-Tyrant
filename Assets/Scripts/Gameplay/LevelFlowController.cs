// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Gestiona flujo de 3 niveles, transiciones y boss final.
using System.Collections;
using TMPro;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
    public event System.Action<int> CurrentLevelChanged;
    public event System.Action<int> RemainingEnemiesChanged;

    [Header("Referencias")]
    [SerializeField] private PlayerEvolutionController playerEvolution;
    [SerializeField] private PlayerRespawnController playerRespawn;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameOverPresenter gameOverPresenter;
    [SerializeField] private GameObject levelTransitionPanel;
    [SerializeField] private TMP_Text levelTransitionText;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Transicion")]
    [SerializeField] private float transitionSeconds = 5f;
    [SerializeField] private bool enableDebugLogs = true;

    [Header("Nivel 1")]
    [SerializeField] private float level1SpawnInterval = 1.5f;
    [SerializeField] private int level1MaxAliveEnemies = 20;
    [SerializeField] private int level1EnemyHealth = 1;
    [SerializeField] private float level1EnemySpeed = 2f;
    [SerializeField] private Color level1EnemyColor = Color.white;

    [Header("Nivel 2")]
    [SerializeField] private float level2SpawnInterval = 1.1f;
    [SerializeField] private int level2MaxAliveEnemies = 24;
    [SerializeField] private int level2EnemyHealth = 2;
    [SerializeField] private float level2EnemySpeed = 2.8f;
    [SerializeField] private Color level2EnemyColor = new Color(1f, 0.8f, 0.5f);

    [Header("Nivel 3")]
    [SerializeField] private float level3SpawnInterval = 0.8f;
    [SerializeField] private int level3MaxAliveEnemies = 28;
    [SerializeField] private int level3EnemyHealth = 3;
    [SerializeField] private float level3EnemySpeed = 3.2f;
    [SerializeField] private Color level3EnemyColor = new Color(1f, 0.5f, 0.5f);

    [Header("Boss Final")]
    [SerializeField] private int bossSpawnKills = -1;
    [SerializeField] private int bossHealth = 40;
    [SerializeField] private float bossSpeed = 1f;
    [SerializeField] private float bossScaleMultiplier = 2.5f;
    [SerializeField] private Color bossColor = new Color(0.7f, 0.2f, 0.9f);

    private int currentLevel = 1;
    private bool isTransitioning;
    private bool bossSpawned;
    private bool bossPendingSpawn;
    private bool roundEnded;
    private EnemyHealth bossHealthComponent;
    private SimpleEnemyMover bossMover;

    private void OnEnable()
    {
        if (playerEvolution != null)
        {
            playerEvolution.StateChanged += OnStateChanged;
        }

        EnemyHealth.EnemyKilledDetailed += OnEnemyKilledDetailed;
        SimpleEnemyMover.EnemyLeftArena += OnEnemyLeftArena;
    }

    private void Start()
    {
        if (levelTransitionPanel != null)
        {
            levelTransitionPanel.SetActive(false);
        }

        if (bossSpawnKills <= 0 && playerEvolution != null)
        {
            bossSpawnKills = Mathf.Max(1, playerEvolution.KillsToState3 * 2);
        }

        ApplyLevelSettings(1);
        NotifyRemainingEnemies();
        Log("Start -> Nivel 1 inicializado. Boss spawn kills: " + bossSpawnKills);
    }

    private void Update()
    {
        if (playerEvolution == null || enemySpawner == null)
        {
            return;
        }

        if (currentLevel == 3 && !isTransitioning && !bossSpawned && !bossPendingSpawn && playerEvolution.TotalKills >= bossSpawnKills)
        {
            bossPendingSpawn = true;
            enemySpawner.SetSpawningEnabled(false);
            Log("Se alcanzó objetivo para boss. Se detiene spawn, esperando limpiar enemigos restantes.");
            TrySpawnBossWhenArenaCleared();
        }

        // Evita quedar trabado cuando el último enemigo se destruye al final del frame.
        if (bossPendingSpawn && !bossSpawned)
        {
            TrySpawnBossWhenArenaCleared();
        }

        NotifyRemainingEnemies();
    }

    private void OnDisable()
    {
        if (playerEvolution != null)
        {
            playerEvolution.StateChanged -= OnStateChanged;
        }

        EnemyHealth.EnemyKilledDetailed -= OnEnemyKilledDetailed;
        SimpleEnemyMover.EnemyLeftArena -= OnEnemyLeftArena;
    }

    private void OnStateChanged(EvolutionState newState)
    {
        if (isTransitioning)
        {
            return;
        }

        if (newState == EvolutionState.State2 && currentLevel < 2)
        {
            StartCoroutine(TransitionToLevelRoutine(2));
        }
        else if (newState == EvolutionState.State3 && currentLevel < 3)
        {
            StartCoroutine(TransitionToLevelRoutine(3));
        }
    }

    private IEnumerator TransitionToLevelRoutine(int targetLevel)
    {
        isTransitioning = true;
        Log("Comienza transición al nivel " + targetLevel);

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(false);
            enemySpawner.ClearAliveEnemies();
        }

        playerRespawn?.RespawnNow();
        ShowLevelTransition(targetLevel);

        float timer = transitionSeconds;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        HideLevelTransition();
        ApplyLevelSettings(targetLevel);
        isTransitioning = false;
        NotifyRemainingEnemies();
        Log("Termina transición. Nivel activo: " + currentLevel);
    }

    private void ApplyLevelSettings(int level)
    {
        if (enemySpawner == null)
        {
            return;
        }

        currentLevel = Mathf.Clamp(level, 1, 3);
        switch (currentLevel)
        {
            case 1:
                enemySpawner.SetSpawnInterval(level1SpawnInterval);
                enemySpawner.SetMaxAliveEnemies(level1MaxAliveEnemies);
                enemySpawner.SetRuntimeEnemyStats(level1EnemyHealth, level1EnemySpeed, false, level1EnemyColor);
                enemySpawner.SetSpawningEnabled(true);
                break;

            case 2:
                enemySpawner.SetSpawnInterval(level2SpawnInterval);
                enemySpawner.SetMaxAliveEnemies(level2MaxAliveEnemies);
                enemySpawner.SetRuntimeEnemyStats(level2EnemyHealth, level2EnemySpeed, false, level2EnemyColor);
                enemySpawner.SetSpawningEnabled(true);
                break;

            default:
                enemySpawner.SetSpawnInterval(level3SpawnInterval);
                enemySpawner.SetMaxAliveEnemies(level3MaxAliveEnemies);
                enemySpawner.SetRuntimeEnemyStats(level3EnemyHealth, level3EnemySpeed, false, level3EnemyColor);
                enemySpawner.SetSpawningEnabled(true);
                break;
        }

        bossPendingSpawn = false;
        CurrentLevelChanged?.Invoke(currentLevel);
    }

    private void SpawnFinalBoss()
    {
        if (enemySpawner == null)
        {
            return;
        }

        enemySpawner.SetSpawningEnabled(false);

        Vector3 spawnPos = ResolveBossSpawnPosition();
        GameObject boss = enemySpawner.SpawnAtPosition(spawnPos, bossHealth, bossSpeed, true, bossColor);
        if (boss == null)
        {
            Log("No se pudo crear el boss final.");
            return;
        }

        boss.transform.localScale *= bossScaleMultiplier;
        bossHealthComponent = boss.GetComponent<EnemyHealth>();
        bossMover = boss.GetComponent<SimpleEnemyMover>();
        bossSpawned = true;
        bossPendingSpawn = false;
        NotifyRemainingEnemies();
        Log("Boss final spawneado.");
    }

    private Vector3 ResolveBossSpawnPosition()
    {
        if (bossSpawnPoint != null)
        {
            return bossSpawnPoint.position;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            return Vector3.zero;
        }

        Vector3 world = cam.ViewportToWorldPoint(new Vector3(0.5f, 1.05f, 0f));
        world.z = 0f;
        return world;
    }

    private void OnEnemyKilledDetailed(EnemyHealth killedEnemy)
    {
        if (!bossSpawned || killedEnemy == null)
        {
            return;
        }

        if (killedEnemy == bossHealthComponent)
        {
            Log("Boss final eliminado.");
            roundEnded = true;
            NotifyRemainingEnemies();
            gameOverPresenter?.TriggerGameOverFromExternal("GANASTE");
            return;
        }

        TrySpawnBossWhenArenaCleared();
        NotifyRemainingEnemies();
    }

    private void OnEnemyLeftArena(SimpleEnemyMover mover)
    {
        if (mover == null)
        {
            return;
        }

        if (bossSpawned && mover == bossMover)
        {
            Log("Boss final escapó del área.");
            roundEnded = true;
            NotifyRemainingEnemies();
            gameOverPresenter?.TriggerGameOverFromExternal("GAME OVER");
            return;
        }

        TrySpawnBossWhenArenaCleared();
        NotifyRemainingEnemies();
    }

    private void ShowLevelTransition(int targetLevel)
    {
        if (levelTransitionPanel != null)
        {
            levelTransitionPanel.SetActive(true);
        }

        if (levelTransitionText != null)
        {
            levelTransitionText.text = "Level " + targetLevel + "-3";
        }
    }

    private void HideLevelTransition()
    {
        if (levelTransitionPanel != null)
        {
            levelTransitionPanel.SetActive(false);
        }
    }

    private void TrySpawnBossWhenArenaCleared()
    {
        if (!bossPendingSpawn || enemySpawner == null || bossSpawned)
        {
            return;
        }

        if (enemySpawner.CountAliveRegularEnemies() <= 0)
        {
            SpawnFinalBoss();
        }
    }

    private void NotifyRemainingEnemies()
    {
        int remaining = 0;
        if (roundEnded)
        {
            remaining = 0;
        }
        else if (bossSpawned)
        {
            remaining = bossHealthComponent != null ? 1 : 0;
        }
        else if (currentLevel == 1 && playerEvolution != null)
        {
            remaining = Mathf.Max(0, playerEvolution.KillsToState2 - playerEvolution.CurrentKills);
        }
        else if (currentLevel == 2 && playerEvolution != null)
        {
            remaining = Mathf.Max(0, playerEvolution.KillsToState3 - playerEvolution.CurrentKills);
        }
        else if (currentLevel == 3 && playerEvolution != null)
        {
            if (bossPendingSpawn && enemySpawner != null)
            {
                remaining = Mathf.Max(0, enemySpawner.CountAliveRegularEnemies());
            }
            else
            {
                remaining = Mathf.Max(0, bossSpawnKills - playerEvolution.TotalKills);
            }
        }

        RemainingEnemiesChanged?.Invoke(remaining);
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[LevelFlowController] " + message, this);
        }
    }
}
