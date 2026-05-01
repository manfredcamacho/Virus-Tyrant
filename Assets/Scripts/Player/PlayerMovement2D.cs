// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Mueve al jugador y limita su posicion a los bordes de camara con padding.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private MonoBehaviour inputReaderBehaviour;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float screenPadding = 0f;
    [SerializeField] private bool autoPaddingFromVisualBounds = true;
    [SerializeField] private bool enableDebugLogs = true;

    private IMovementInputReader inputReader;
    private Rigidbody2D rb;
    private Camera cachedCamera;
    private float paddingX;
    private float paddingY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cachedCamera = Camera.main;
        inputReader = inputReaderBehaviour as IMovementInputReader;
        RecalculatePadding();
        Log("Awake -> paddingX: " + paddingX + ", paddingY: " + paddingY);
    }

    private void OnValidate()
    {
        screenPadding = Mathf.Max(0f, screenPadding);
        speed = Mathf.Max(0f, speed);
    }

    private void Start()
    {
        RecalculatePadding();
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

        position.x = Mathf.Clamp(position.x, min.x + paddingX, max.x - paddingX);
        position.y = Mathf.Clamp(position.y, min.y + paddingY, max.y - paddingY);
        transform.position = position;
    }

    private void RecalculatePadding()
    {
        paddingX = screenPadding;
        paddingY = screenPadding;
        if (!autoPaddingFromVisualBounds)
        {
            return;
        }

        Renderer rendererComponent = GetComponentInChildren<Renderer>();
        if (rendererComponent == null)
        {
            return;
        }

        Vector3 extents = rendererComponent.bounds.extents;
        paddingX = Mathf.Max(paddingX, extents.x);
        paddingY = Mathf.Max(paddingY, extents.y);
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[PlayerMovement2D] " + message, this);
        }
    }
}
