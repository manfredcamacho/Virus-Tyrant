// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Aplica daño al jugador cuando hay colision.
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyDamageDealer : MonoBehaviour
{
    [SerializeField] private int contactDamage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(contactDamage);
        }
    }
}
