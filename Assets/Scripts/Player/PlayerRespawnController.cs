// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Reubica al jugador en un punto de respawn y limpia su velocidad.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRespawnController : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void RespawnNow()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
