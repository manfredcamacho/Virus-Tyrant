// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Instancia proyectiles desde un prefab y punto de disparo.
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector2 fireDirection = Vector2.up;
    [SerializeField] private int baseDamage = 1;

    public void Spawn()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }

        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.Initialize(fireDirection, baseDamage);
    }
}
