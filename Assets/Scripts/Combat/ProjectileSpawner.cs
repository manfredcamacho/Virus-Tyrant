// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Instancia proyectiles desde un prefab y punto de disparo.
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector2 fireDirection = Vector2.up;
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private bool enableDebugLogs = true;

    public void Spawn()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Log("Spawn cancelado: projectilePrefab o firePoint sin asignar.");
            return;
        }

        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.Initialize(fireDirection, baseDamage);
        Log("Projectile creado en " + firePoint.position + " dir " + fireDirection + " dmg " + baseDamage);
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[ProjectileSpawner] " + message, this);
        }
    }
}
