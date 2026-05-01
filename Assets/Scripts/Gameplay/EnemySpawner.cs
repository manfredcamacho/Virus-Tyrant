// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Genera enemigos periodicamente en puntos definidos.
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int maxAliveEnemies = 20;
    [SerializeField] private bool enableDebugLogs = true;

    private float timer;
    private bool spawningEnabled = true;
    private int runtimeEnemyHealth = -1;
    private float runtimeEnemySpeed = -1f;
    private bool runtimeBossFlag;
    private bool runtimeColorOverride;
    private Color runtimeEnemyColor = Color.white;

    private void Update()
    {
        if (!spawningEnabled)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0f)
        {
            return;
        }

        timer = spawnInterval;
        SpawnEnemyIfPossible();
    }

    private void SpawnEnemyIfPossible()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Log("No se puede spawnear: falta enemyPrefab o spawnPoints.");
            return;
        }

        EnemyHealth[] aliveEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        if (aliveEnemies.Length >= maxAliveEnemies)
        {
            Log("Límite alcanzado. Enemigos vivos: " + aliveEnemies.Length + "/" + maxAliveEnemies);
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];
        SpawnConfiguredEnemyAt(spawnPoint.position);
        Log("Spawn enemigo en punto #" + index + " (" + spawnPoint.position + "). Vivos: " + (aliveEnemies.Length + 1));
    }

    public void SetSpawningEnabled(bool enabled)
    {
        spawningEnabled = enabled;
        Log("SetSpawningEnabled -> " + spawningEnabled);
    }

    public void SetSpawnInterval(float newInterval)
    {
        spawnInterval = Mathf.Max(0.05f, newInterval);
        Log("SetSpawnInterval -> " + spawnInterval);
    }

    public void SetMaxAliveEnemies(int newMaxAlive)
    {
        maxAliveEnemies = Mathf.Max(1, newMaxAlive);
        Log("SetMaxAliveEnemies -> " + maxAliveEnemies);
    }

    public void SetRuntimeEnemyStats(int health, float speed, bool isBoss, Color color, bool overrideColor = true)
    {
        runtimeEnemyHealth = health;
        runtimeEnemySpeed = speed;
        runtimeBossFlag = isBoss;
        runtimeEnemyColor = color;
        runtimeColorOverride = overrideColor;
        Log("SetRuntimeEnemyStats -> hp: " + health + ", speed: " + speed + ", boss: " + isBoss + ", colorOverride: " + overrideColor);
    }

    public void ClearAliveEnemies(bool includeBosses = false)
    {
        EnemyHealth[] aliveEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        for (int i = 0; i < aliveEnemies.Length; i++)
        {
            EnemyHealth enemy = aliveEnemies[i];
            if (enemy == null)
            {
                continue;
            }

            if (!includeBosses && enemy.IsBossEnemy)
            {
                continue;
            }

            Destroy(enemy.gameObject);
        }
        Log("ClearAliveEnemies -> includeBosses: " + includeBosses + ", encontrados: " + aliveEnemies.Length);
    }

    public GameObject SpawnAtPosition(Vector3 worldPosition, int health, float speed, bool isBoss)
    {
        runtimeEnemyHealth = health;
        runtimeEnemySpeed = speed;
        runtimeBossFlag = isBoss;
        GameObject instance = SpawnConfiguredEnemyAt(worldPosition);
        runtimeBossFlag = false;
        return instance;
    }

    public GameObject SpawnAtPosition(Vector3 worldPosition, int health, float speed, bool isBoss, Color color)
    {
        runtimeEnemyHealth = health;
        runtimeEnemySpeed = speed;
        runtimeBossFlag = isBoss;
        runtimeEnemyColor = color;
        runtimeColorOverride = true;
        GameObject instance = SpawnConfiguredEnemyAt(worldPosition);
        runtimeBossFlag = false;
        return instance;
    }

    public int CountAliveRegularEnemies()
    {
        EnemyHealth[] aliveEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        int count = 0;
        for (int i = 0; i < aliveEnemies.Length; i++)
        {
            EnemyHealth enemy = aliveEnemies[i];
            if (enemy != null && !enemy.IsBossEnemy)
            {
                count++;
            }
        }

        return count;
    }

    private GameObject SpawnConfiguredEnemyAt(Vector3 worldPosition)
    {
        if (enemyPrefab == null)
        {
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, worldPosition, Quaternion.identity);
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            if (runtimeEnemyHealth > 0)
            {
                enemyHealth.ConfigureMaxHealth(runtimeEnemyHealth);
            }
            enemyHealth.SetBossFlag(runtimeBossFlag);
        }

        SimpleEnemyMover mover = enemy.GetComponent<SimpleEnemyMover>();
        if (mover != null && runtimeEnemySpeed >= 0f)
        {
            mover.ConfigureSpeed(runtimeEnemySpeed);
        }

        if (runtimeColorOverride)
        {
            SpriteRenderer spriteRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = runtimeEnemyColor;
            }
        }

        return enemy;
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[EnemySpawner] " + message, this);
        }
    }
}
