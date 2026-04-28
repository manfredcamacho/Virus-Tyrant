// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
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

    private IMovementInputReader inputReader;
    private float fireCooldown;

    private void Awake()
    {
        inputReader = inputReaderBehaviour as IMovementInputReader;
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

        if (centerSpawner != null)
        {
            centerSpawner.Spawn();
        }

        bool evolved = evolutionController != null && evolutionController.CurrentState == EvolutionState.State2;
        if (evolved)
        {
            leftSpawner?.Spawn();
            rightSpawner?.Spawn();
        }

        fireCooldown = fireInterval;
    }
}
