// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Gestiona vida del enemigo y notifica kills al morir.
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static event Action EnemyKilled;
    public static event Action<EnemyHealth> EnemyKilledDetailed;

    [SerializeField] private int maxHealth = 1;
    [SerializeField] private bool isBossEnemy;
    [SerializeField] private bool enableDebugLogs = true;

    private int currentHealth;
    public bool IsBossEnemy => isBossEnemy;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
        Log("Awake -> vida inicial: " + currentHealth);
    }

    public void TakeDamage(int damageAmount = 1)
    {
        currentHealth -= Mathf.Max(1, damageAmount);
        Log("Daño recibido: " + damageAmount + ". Vida actual: " + currentHealth);
        if (currentHealth <= 0)
        {
            EnemyKilled?.Invoke();
            EnemyKilledDetailed?.Invoke(this);
            Log("Enemy eliminado. Se emite EnemyKilled.");
            Destroy(gameObject);
        }
    }

    public void ConfigureMaxHealth(int newMaxHealth)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);
        currentHealth = maxHealth;
        Log("ConfigureMaxHealth -> " + maxHealth);
    }

    public void SetBossFlag(bool boss)
    {
        isBossEnemy = boss;
        Log("SetBossFlag -> " + isBossEnemy);
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[EnemyHealth] " + message, this);
        }
    }
}
