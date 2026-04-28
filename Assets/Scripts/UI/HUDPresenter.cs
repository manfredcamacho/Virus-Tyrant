// Autor: Pathogen Zero Team
// Email: dev@pathogenzero.local
// Funcion: Sincroniza HUD con vidas, estado evolutivo y progreso de evolucion.
using UnityEngine;
using UnityEngine.UI;

public class HUDPresenter : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealth;
    [SerializeField] private PlayerEvolutionController playerEvolution;
    [SerializeField] private Text livesText;
    [SerializeField] private Text stateText;
    [SerializeField] private Text killsProgressText;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.LivesChanged += OnLivesChanged;
        }

        if (playerEvolution != null)
        {
            playerEvolution.StateChanged += OnStateChanged;
            playerEvolution.KillsProgressChanged += OnKillsProgressChanged;
        }
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            OnLivesChanged(playerHealth.CurrentLives);
        }

        if (playerEvolution != null)
        {
            OnStateChanged(playerEvolution.CurrentState);
            OnKillsProgressChanged(playerEvolution.CurrentKills, playerEvolution.KillsToEvolve);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.LivesChanged -= OnLivesChanged;
        }

        if (playerEvolution != null)
        {
            playerEvolution.StateChanged -= OnStateChanged;
            playerEvolution.KillsProgressChanged -= OnKillsProgressChanged;
        }
    }

    private void OnLivesChanged(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }

    private void OnStateChanged(EvolutionState evolutionState)
    {
        if (stateText != null)
        {
            stateText.text = evolutionState == EvolutionState.State2 ? "State: 2" : "State: 1";
        }
    }

    private void OnKillsProgressChanged(int current, int required)
    {
        if (killsProgressText != null)
        {
            killsProgressText.text = "Evolve: " + current + "/" + required;
        }
    }
}
