// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Gestiona vida del enemigo y notifica kills al morir.
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static event Action EnemyKilled;

    [SerializeField] private int maxHealth = 1;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void TakeDamage(int damageAmount = 1)
    {
        currentHealth -= Mathf.Max(1, damageAmount);
        if (currentHealth <= 0)
        {
            EnemyKilled?.Invoke();
            Destroy(gameObject);
        }
    }
}
