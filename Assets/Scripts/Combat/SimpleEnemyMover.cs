// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Mueve al enemigo en linea recta hacia abajo en vista top-view.
using UnityEngine;

public class SimpleEnemyMover : MonoBehaviour
{
    [SerializeField] private Vector2 direction = Vector2.down;
    [SerializeField] private float speed = 2f;

    private void Update()
    {
        transform.position += (Vector3)(direction.normalized * (speed * Time.deltaTime));
    }
}
