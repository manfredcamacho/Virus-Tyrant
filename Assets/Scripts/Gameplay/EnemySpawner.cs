// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Genera enemigos periodicamente en puntos definidos.
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int maxAliveEnemies = 20;

    private float timer;

    private void Update()
    {
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
            return;
        }

        EnemyHealth[] aliveEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        if (aliveEnemies.Length >= maxAliveEnemies)
        {
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }
}
