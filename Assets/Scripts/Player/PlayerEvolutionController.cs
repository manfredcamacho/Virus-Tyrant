// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Gestiona evolucion/involucion del jugador segun kills y daño recibido.
using System;
using UnityEngine;

public enum EvolutionState
{
    Base = 0,
    State2 = 1,
    State3 = 2
}

public class PlayerEvolutionController : MonoBehaviour
{
    [SerializeField] private int killsToState2 = 10;
    [SerializeField] private int killsToState3 = 25;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Sprite state2Sprite;
    [SerializeField] private Sprite state3Sprite;
    [SerializeField] private bool enableDebugLogs = true;

    public event Action<EvolutionState> StateChanged;
    public event Action<int, int> KillsProgressChanged;

    public EvolutionState CurrentState { get; private set; } = EvolutionState.Base;
    public int CurrentKills { get; private set; }
    public int TotalKills { get; private set; }
    public int KillsToState2 => killsToState2;
    public int KillsToState3 => killsToState3;
    public int KillsToNextEvolution
    {
        get
        {
            return CurrentState switch
            {
                EvolutionState.Base => killsToState2,
                EvolutionState.State2 => killsToState3,
                _ => killsToState3
            };
        }
    }

    private void Awake()
    {
        // Fuerza estado inicial consistente aunque el objeto se reutilice.
        CurrentState = EvolutionState.Base;
        CurrentKills = 0;
        TotalKills = 0;
        Log("Awake -> Estado inicial Base, kills en 0.");
    }

    private void Start()
    {
        ApplyVisualForState();
        NotifyProgress();
        StateChanged?.Invoke(CurrentState);
        Log("Start -> Estado inicial notificado al HUD.");
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
        // En evolución máxima, el contador queda clavado en el tope.
        TotalKills++;
        if (CurrentState == EvolutionState.State3 && CurrentKills >= killsToState3)
        {
            return;
        }

        CurrentKills++;
        CurrentKills = Mathf.Min(CurrentKills, killsToState3);
        Log("Kill registrada. Progreso: " + CurrentKills + "/" + KillsToNextEvolution);
        NotifyProgress();

        if (CurrentState == EvolutionState.Base && CurrentKills >= killsToState2)
        {
            EvolveToState2();
        }
        else if (CurrentState == EvolutionState.State2 && CurrentKills >= killsToState3)
        {
            EvolveToState3();
        }
    }

    public void EvolveToState2()
    {
        if (CurrentState != EvolutionState.Base)
        {
            return;
        }

        CurrentState = EvolutionState.State2;
        ApplyVisualForState();
        StateChanged?.Invoke(CurrentState);
        Log("Evoluciono a State2.");
        NotifyProgress();
    }

    public void EvolveToState3()
    {
        if (CurrentState != EvolutionState.State2)
        {
            return;
        }

        CurrentState = EvolutionState.State3;
        ApplyVisualForState();
        StateChanged?.Invoke(CurrentState);
        Log("Evoluciono a State3.");
        NotifyProgress();
    }

    public void RegressOneLevel()
    {
        if (CurrentState == EvolutionState.Base && CurrentKills == 0)
        {
            return;
        }

        CurrentState = CurrentState switch
        {
            EvolutionState.State3 => EvolutionState.State2,
            EvolutionState.State2 => EvolutionState.Base,
            _ => EvolutionState.Base
        };

        // Al involucionar, vuelve al mínimo de la evolución anterior.
        CurrentKills = CurrentState switch
        {
            EvolutionState.State2 => killsToState2,
            _ => 0
        };

        ApplyVisualForState();
        StateChanged?.Invoke(CurrentState);
        NotifyProgress();
        Log("Regresa un nivel. Estado actual: " + CurrentState);
    }

    public void RegressToBase()
    {
        if (CurrentState == EvolutionState.Base && CurrentKills == 0)
        {
            return;
        }

        CurrentState = EvolutionState.Base;
        CurrentKills = 0;
        ApplyVisualForState();
        StateChanged?.Invoke(CurrentState);
        NotifyProgress();
        Log("Regreso a Base y reseteo kills.");
    }

    private void NotifyProgress()
    {
        KillsProgressChanged?.Invoke(CurrentKills, KillsToNextEvolution);
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[PlayerEvolutionController] " + message, this);
        }
    }

    private void ApplyVisualForState()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sprite = CurrentState switch
        {
            EvolutionState.State3 => state3Sprite,
            EvolutionState.State2 => state2Sprite,
            _ => baseSprite
        };
    }
}
