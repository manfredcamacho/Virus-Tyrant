// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Mueve al jugador y limita su posicion a los bordes de camara con padding.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private MonoBehaviour inputReaderBehaviour;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float screenPadding = 0f;

    private IMovementInputReader inputReader;
    private Rigidbody2D rb;
    private Camera cachedCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cachedCamera = Camera.main;
        inputReader = inputReaderBehaviour as IMovementInputReader;
    }

    private void OnValidate()
    {
        screenPadding = Mathf.Max(0f, screenPadding);
        speed = Mathf.Max(0f, speed);
    }

    private void FixedUpdate()
    {
        if (inputReader == null || cachedCamera == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 moveInput = inputReader.ReadMoveInput();
        rb.linearVelocity = moveInput.normalized * speed;

        ClampInsideCameraBounds();
    }

    private void ClampInsideCameraBounds()
    {
        Vector3 position = transform.position;
        Vector3 min = cachedCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 max = cachedCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        position.x = Mathf.Clamp(position.x, min.x + screenPadding, max.x - screenPadding);
        position.y = Mathf.Clamp(position.y, min.y + screenPadding, max.y - screenPadding);
        transform.position = position;
    }
}
