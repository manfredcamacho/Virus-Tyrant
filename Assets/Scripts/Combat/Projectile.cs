// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Mueve un proyectil y aplica daño al colisionar.
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeSeconds = 4f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool enableDebugLogs = true;

    private Vector2 moveDirection = Vector2.up;
    private float lifeTimer;

    public void Initialize(Vector2 direction, int overrideDamage)
    {
        moveDirection = direction.normalized;
        damage = overrideDamage;
        lifeTimer = lifeSeconds;
        Log("Initialize -> direction: " + moveDirection + ", damage: " + damage + ", life: " + lifeSeconds);
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * (speed * Time.deltaTime));

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Log("Impacto contra: " + other.name + ". Aplica daño: " + damage);
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Log("Colisión sin IDamageable con: " + other.name);
        }
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[Projectile] " + message, this);
        }
    }
}
