// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Mueve al enemigo en linea recta hacia abajo en vista top-view.
using UnityEngine;

public class SimpleEnemyMover : MonoBehaviour
{
    public static event System.Action<SimpleEnemyMover> EnemyLeftArena;

    [SerializeField] private Vector2 direction = Vector2.down;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float despawnViewportMargin = 0.2f;
    [SerializeField] private bool enableDebugLogs = true;

    private Camera cachedCamera;

    private void Awake()
    {
        cachedCamera = Camera.main;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction.normalized * (speed * Time.deltaTime));
        TryDespawnOutsideCamera();
    }

    private void TryDespawnOutsideCamera()
    {
        if (cachedCamera == null)
        {
            return;
        }

        Vector3 viewport = cachedCamera.WorldToViewportPoint(transform.position);
        Vector2 moveDir = direction.normalized;

        // Solo destruye cuando cruza el borde de salida respecto de su movimiento.
        bool outside = false;
        if (moveDir.y < 0f)
        {
            outside = viewport.y < -despawnViewportMargin;
        }
        else if (moveDir.y > 0f)
        {
            outside = viewport.y > 1f + despawnViewportMargin;
        }
        else if (moveDir.x < 0f)
        {
            outside = viewport.x < -despawnViewportMargin;
        }
        else if (moveDir.x > 0f)
        {
            outside = viewport.x > 1f + despawnViewportMargin;
        }
        else
        {
            // Fallback por seguridad si direction queda en cero.
            outside = viewport.x < -despawnViewportMargin
                      || viewport.x > 1f + despawnViewportMargin
                      || viewport.y < -despawnViewportMargin
                      || viewport.y > 1f + despawnViewportMargin;
        }

        if (outside)
        {
            Log("Enemy fuera de cámara -> destroy.");
            EnemyLeftArena?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void ConfigureSpeed(float newSpeed)
    {
        speed = Mathf.Max(0f, newSpeed);
        Log("ConfigureSpeed -> " + speed);
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[SimpleEnemyMover] " + message, this);
        }
    }
}
