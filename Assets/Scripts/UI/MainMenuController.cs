// Autor: Manfred Camacho
// Email: manfred.camacho.dev@gmail.com
// Funcion: Controla botones del menu principal (iniciar y salir)
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "SampleScene";
    [SerializeField] private bool enableDebugLogs = true;

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Log("No se puede iniciar: gameplaySceneName vacío.");
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ExitGame()
    {
        Log("Saliendo de la aplicación...");
        Application.Quit();
    }

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log("[MainMenuController] " + message, this);
        }
    }
}
