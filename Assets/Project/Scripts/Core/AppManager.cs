using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages application-level functionality: scene loading, UI panels, game state transitions
/// Does not manage gameplay data - that's handled by GameManager
/// </summary>
public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    [Header("Game State")]
    public GameState currentState = GameState.MainMenu;
    
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject resultPanel;

    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeApp();
    }

    private void InitializeApp()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void ChangeGameState(GameState newState)
    {
        if (currentState == newState) return;

        GameState previousState = currentState;
        currentState = newState;

        Debug.Log($"Game state changed from {previousState} to {newState}");

        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.MainMenu:
                ShowMainMenu();
                break;
            case GameState.Gameplay:
                StartGameplay();
                break;
            case GameState.Result:
                ShowResult();
                break;
        }
    }

    private void ShowMainMenu()
    {
        Time.timeScale = 1f;
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // Load main menu scene if needed
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void StartGameplay()
    {
        Time.timeScale = 1f;
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // Load gameplay scene if needed
        if (SceneManager.GetActiveScene().name != "Gameplay")
        {
            SceneManager.LoadScene("Gameplay");
        }

        // Notify GameManager to initialize gameplay data
        GameManager.Instance?.InitializeGameplayData();
    }

    private void ShowResult()
    {
        Time.timeScale = 0f;
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (resultPanel != null)
            resultPanel.SetActive(true);

        // Notify GameManager to calculate results
        GameManager.Instance?.CalculateGameResults();
    }

    public void StartNewGame()
    {
        ChangeGameState(GameState.Gameplay);
    }

    public void ReturnToMainMenu()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void EndGame()
    {
        ChangeGameState(GameState.Result);
    }

    public void RestartGame()
    {
        ChangeGameState(GameState.MainMenu);
        StartNewGame();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

public enum GameState
{
    MainMenu,
    Gameplay,
    Result
}
