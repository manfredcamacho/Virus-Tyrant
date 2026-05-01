// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Aplica daño al jugador cuando hay colision.
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyDamageDealer : MonoBehaviour
{
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private bool enableDebugLogs = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Log("Contacto con " + other.name + ". Daño: " + contactDamage);
            damageable.TakeDamage(contactDamage);
        }
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[EnemyDamageDealer] " + message, this);
        }
    }
}
