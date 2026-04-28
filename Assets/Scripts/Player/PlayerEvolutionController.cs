// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Gestiona evolucion/involucion del jugador segun kills y daño recibido.
using System;
using UnityEngine;

public enum EvolutionState
{
    State1 = 0,
    State2 = 1
}

public class PlayerEvolutionController : MonoBehaviour
{
    [SerializeField] private int killsToEvolve = 10;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite state1Sprite;
    [SerializeField] private Sprite state2Sprite;

    public event Action<EvolutionState> StateChanged;
    public event Action<int, int> KillsProgressChanged;

    public EvolutionState CurrentState { get; private set; } = EvolutionState.State1;
    public int CurrentKills { get; private set; }
    public int KillsToEvolve => killsToEvolve;

    private void Start()
    {
        ApplyVisualForState();
        NotifyProgress();
    }

    private void OnEnable()
    {
        EnemyHealth.EnemyKilled += RegisterKill;
    }

    private void OnDisable()
    {
        EnemyHealth.EnemyKilled -= RegisterKill;
    }

    public void RegisterKill()
    {
        CurrentKills++;
        NotifyProgress();

        if (CurrentState == EvolutionState.State1 && CurrentKills >= killsToEvolve)
        {
            Evolve();
        }
    }

    public void Evolve()
    {
        CurrentState = EvolutionState.State2;
        ApplyVisualForState();
        StateChanged?.Invoke(CurrentState);
    }

    public void Regress()
    {
        CurrentState = EvolutionState.State1;
        CurrentKills = 0;
        ApplyVisualForState();
        StateChanged?.Invoke(CurrentState);
        NotifyProgress();
    }

    private void NotifyProgress()
    {
        KillsProgressChanged?.Invoke(CurrentKills, killsToEvolve);
    }

    private void ApplyVisualForState()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = CurrentState == EvolutionState.State2
            ? state2Sprite
            : state1Sprite;
    }
}
