// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Aplica dano segun estado evolutivo, resta vidas y ejecuta respawn.
using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float respawnInvulnerabilitySeconds = 1f;
    [SerializeField] private PlayerEvolutionController evolutionController;
    [SerializeField] private PlayerRespawnController respawnController;
    [SerializeField] private bool enableDebugLogs = true;

    public event Action<int> LivesChanged;
    public event Action PlayerDied;

    public int CurrentLives { get; private set; }
    public bool IsDead { get; private set; }

    private bool isInvulnerable;

    private void Awake()
    {
        CurrentLives = maxLives;
        LivesChanged?.Invoke(CurrentLives);
        Log("Awake -> vidas iniciales: " + CurrentLives);
    }

    public void TakeDamage(int damageAmount = 1)
    {
        if (IsDead || isInvulnerable)
        {
            Log("Daño ignorado (muerto o invulnerable).");
            return;
        }

        if (evolutionController != null && evolutionController.CurrentState != EvolutionState.Base)
        {
            Log("Daño recibido en estado evolucionado -> regresa un nivel sin perder vida.");
            evolutionController.RegressOneLevel();
            return;
        }

        CurrentLives -= Mathf.Max(1, damageAmount);
        CurrentLives = Mathf.Max(0, CurrentLives);
        LivesChanged?.Invoke(CurrentLives);
        Log("Daño aplicado. Vidas restantes: " + CurrentLives);

        if (CurrentLives <= 0)
        {
            IsDead = true;
            PlayerDied?.Invoke();
            gameObject.SetActive(false);
            Log("Player murió. Se desactiva objeto.");
            return;
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isInvulnerable = true;
        Log("Respawn iniciado. Invulnerable por " + respawnInvulnerabilitySeconds + "s.");
        if (respawnController != null)
        {
            respawnController.RespawnNow();
        }

        yield return new WaitForSeconds(respawnInvulnerabilitySeconds);
        isInvulnerable = false;
        Log("Invulnerabilidad finalizada.");
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[PlayerHealthController] " + message, this);
        }
    }
}
