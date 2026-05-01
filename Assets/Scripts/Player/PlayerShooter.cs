// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Controla cadencia y patron de disparo segun estado evolutivo.
using UnityEngine;

public class PlayerShooter : MonoBehaviour, IFireController
{
    [SerializeField] private MonoBehaviour inputReaderBehaviour;
    [SerializeField] private ProjectileSpawner centerSpawner;
    [SerializeField] private ProjectileSpawner leftSpawner;
    [SerializeField] private ProjectileSpawner rightSpawner;
    [SerializeField] private PlayerEvolutionController evolutionController;
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField] private bool enableDebugLogs = true;

    private IMovementInputReader inputReader;
    private float fireCooldown;

    private void Awake()
    {
        inputReader = inputReaderBehaviour as IMovementInputReader;
        Log("Awake -> inputReader " + (inputReader != null ? "ok." : "null."));
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (inputReader == null)
        {
            return;
        }

        if (inputReader.IsFirePressed())
        {
            TryFire();
        }
    }

    public void TryFire()
    {
        if (fireCooldown > 0f)
        {
            return;
        }

        EvolutionState currentState = evolutionController != null
            ? evolutionController.CurrentState
            : EvolutionState.Base;

        switch (currentState)
        {
            case EvolutionState.State3:
                centerSpawner?.Spawn();
                leftSpawner?.Spawn();
                rightSpawner?.Spawn();
                Log("Disparo State3 -> 3 balas.");
                break;

            case EvolutionState.State2:
                if (leftSpawner != null && rightSpawner != null)
                {
                    leftSpawner.Spawn();
                    rightSpawner.Spawn();
                    Log("Disparo State2 -> 2 balas.");
                }
                else
                {
                    centerSpawner?.Spawn();
                    Log("Disparo State2 con fallback -> 1 bala (faltan spawners laterales).");
                }
                break;

            default:
                centerSpawner?.Spawn();
                Log("Disparo Base -> 1 bala.");
                break;
        }

        fireCooldown = fireInterval;
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[PlayerShooter] " + message, this);
        }
    }
}
