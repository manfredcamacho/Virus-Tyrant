// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
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

    public event Action<int> LivesChanged;
    public event Action PlayerDied;

    public int CurrentLives { get; private set; }
    public bool IsDead { get; private set; }

    private bool isInvulnerable;

    private void Awake()
    {
        CurrentLives = maxLives;
        LivesChanged?.Invoke(CurrentLives);
    }

    public void TakeDamage(int damageAmount = 1)
    {
        if (IsDead || isInvulnerable)
        {
            return;
        }

        if (evolutionController != null && evolutionController.CurrentState == EvolutionState.State2)
        {
            evolutionController.Regress();
            return;
        }

        CurrentLives -= Mathf.Max(1, damageAmount);
        CurrentLives = Mathf.Max(0, CurrentLives);
        LivesChanged?.Invoke(CurrentLives);

        if (CurrentLives <= 0)
        {
            IsDead = true;
            PlayerDied?.Invoke();
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isInvulnerable = true;
        if (respawnController != null)
        {
            respawnController.RespawnNow();
        }

        yield return new WaitForSeconds(respawnInvulnerabilitySeconds);
        isInvulnerable = false;
    }
}
