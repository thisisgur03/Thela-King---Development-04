/*
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Booting,
    MainMenu,
    Loading,
    LevelStart,
    Playing,
    Paused,
    LevelComplete,
    GameOver,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private UIScreenManager uiSceneManager;

    private GameState currentState;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        uiSceneManager = GetComponent<UIScreenManager>();
    }

    private void Start()
    {
        currentState = GameState.Booting;
        EnterState(GameState.Booting);
    }

    private IEnumerator InitialGameSetup()
    {

        yield return new WaitForSeconds(3f);

        uiSceneManager.ChangeScreen(ScreenType.GameLogo);

        yield return new WaitForSeconds(3f);

        ChangeState(GameState.MainMenu);
    }

    private bool IsValidTransition(GameState newState)
    {
        switch (currentState)
        {
            case GameState.Booting:
                if (newState == GameState.MainMenu)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case GameState.MainMenu:
                if (newState == GameState.Loading)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case GameState.Loading:
                if (newState == GameState.Playing || newState == GameState.MainMenu)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case GameState.LevelStart:
                if (newState == GameState.Playing)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case GameState.Playing:
                if (newState == GameState.Paused || newState == GameState.LevelComplete || newState == GameState.GameOver)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case GameState.Paused:
                if (newState == GameState.Playing || newState == GameState.MainMenu)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case GameState.LevelComplete:
                if (newState == GameState.Loading || newState == GameState.MainMenu || newState == GameState.LevelStart)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case GameState.GameOver:
                if (newState == GameState.Loading || newState == GameState.MainMenu || newState == GameState.LevelStart)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            default:
                return false;
        }
    }

    public void ChangeState(GameState newState)
    {
        if (IsValidTransition(newState))
        {
            ExitState(currentState);

            currentState = newState;

            EnterState(currentState);
        }
        else
        {
            Debug.Log("Not a Valid State Transition");
        }
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Booting:
                StartCoroutine(InitialGameSetup());
                break;

            case GameState.MainMenu:
                uiSceneManager.ChangeScreen(ScreenType.MainMenu);
                break;

            case GameState.Loading:
                uiSceneManager.ChangeScreen(ScreenType.Loading);
                StartCoroutine(LoadingGame());
                ChangeState(GameState.LevelStart);
                break;

            case GameState.LevelStart:
                // Prepare level
                // Reset player
                // Reset level systems
                break;

            case GameState.Playing:
                uiSceneManager.ChangeScreen(ScreenType.GamePlay);
                SceneManager.LoadScene("Main");
                // Enable player controls
                // Start gameplay systems
                break;

            case GameState.Paused:
                uiSceneManager.ChangeScreen(ScreenType.Pause);

                // Pause gameplay
                // Disable player controls
                break;

            case GameState.LevelComplete:
                uiSceneManager.ChangeScreen(ScreenType.LevelComplete);

                // Stop gameplay
                // Show completion/reward logic
                break;

            case GameState.GameOver:
                uiSceneManager.ChangeScreen(ScreenType.GameOver);

                // Stop gameplay
                // Show game-over logic
                break;

            default:
                break;
        }
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Booting:
                // Stop boot-specific processes if needed
                break;

            case GameState.MainMenu:
                // Stop menu-specific processes if needed
                break;

            case GameState.Loading:
                // Stop loading-specific processes if needed
                break;

            case GameState.LevelStart:
                // Clean up level-start logic if needed
                break;

            case GameState.Playing:
                // Disable player controls
                // Stop gameplay systems
                break;

            case GameState.Paused:
                // Resume gameplay
                // Re-enable player controls
                break;

            case GameState.LevelComplete:
                // Stop completion-specific processes if needed
                break;

            case GameState.GameOver:
                // Stop game-over-specific processes if needed
                break;

            default:
                break;
        }
    }

    private IEnumerator LoadingGame()
    {
        yield return new WaitForSeconds(3f);
        ChangeState(GameState.Playing);
    }

    public void LoadGame()
    {
        ChangeState(GameState.Loading);
    }
}
*/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Booting,
    MainMenu,
    Loading,
    LevelStart,
    Playing,
    Paused,
    LevelComplete,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scenes")]
    [SerializeField] private string mainSceneName = "Main";

    [Header("Boot")]
    [SerializeField] private float companyLogoDuration = 3f;
    [SerializeField] private float gameLogoDuration = 3f;

    [Header("Loading")]
    [SerializeField] private float minimumLoadingTime = 1f;

    private GameState currentState;
    private UIScreenManager uiScreenManager;

    private Coroutine stateCoroutine;

    public GameState CurrentState => currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        EnterState(GameState.Booting);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainSceneName &&
            currentState == GameState.Booting)
        {
            ChangeState(GameState.MainMenu);
        }
    }

    public void RegisterUIScreenManager(UIScreenManager manager)
    {
        uiScreenManager = manager;
    }

    
    // --------------------------------------------------
    // STATE TRANSITIONS
    // --------------------------------------------------

    private bool IsValidTransition(GameState newState)
    {
        switch (currentState)
        {
            case GameState.Booting:
                return newState == GameState.MainMenu;

            case GameState.MainMenu:
                return newState == GameState.Loading;

            case GameState.Loading:
                return newState == GameState.LevelStart ||
                       newState == GameState.MainMenu;

            case GameState.LevelStart:
                return newState == GameState.Playing;

            case GameState.Playing:
                return newState == GameState.Paused ||
                       newState == GameState.LevelComplete ||
                       newState == GameState.GameOver;

            case GameState.Paused:
                return newState == GameState.Playing ||
                       newState == GameState.MainMenu;

            case GameState.LevelComplete:
                return newState == GameState.Loading ||
                       newState == GameState.MainMenu;

            case GameState.GameOver:
                return newState == GameState.Loading ||
                       newState == GameState.MainMenu;

            default:
                return false;
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        if (!IsValidTransition(newState))
        {
            Debug.LogWarning(
                $"Invalid GameState transition: {currentState} -> {newState}"
            );

            return;
        }

        ExitState(currentState);

        currentState = newState;

        Debug.Log($"GameState: {currentState}");

        EnterState(currentState);
    }

    // --------------------------------------------------
    // ENTER STATE
    // --------------------------------------------------

    private void EnterState(GameState state)
    {
        StopStateCoroutine();

        switch (state)
        {
            case GameState.Booting:
                stateCoroutine = StartCoroutine(BootSequence());
                break;

            case GameState.MainMenu:
                EnterMainMenu();
                break;

            case GameState.Loading:
                stateCoroutine = StartCoroutine(PrepareGame());
                break;

            case GameState.LevelStart:
                stateCoroutine = StartCoroutine(StartLevel());
                break;

            case GameState.Playing:
                EnterPlaying();
                break;

            case GameState.Paused:
                EnterPaused();
                break;

            case GameState.LevelComplete:
                EnterLevelComplete();
                break;

            case GameState.GameOver:
                EnterGameOver();
                break;
        }
    }

    // --------------------------------------------------
    // EXIT STATE
    // --------------------------------------------------

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Booting:
                break;

            case GameState.MainMenu:
                break;

            case GameState.Loading:
                break;

            case GameState.LevelStart:
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 1f;
                break;

            case GameState.LevelComplete:
                break;

            case GameState.GameOver:
                break;
        }
    }

    // --------------------------------------------------
    // BOOT
    // --------------------------------------------------

    private IEnumerator BootSequence()
    {
        if (uiScreenManager == null)
        {
            Debug.LogError("GameManager: Boot UIScreenManager not registered.");
            yield break;
        }

        uiScreenManager.ChangeScreen(ScreenType.CompanyLogo);

        yield return new WaitForSeconds(companyLogoDuration);

        uiScreenManager.ChangeScreen(ScreenType.GameLogo);

        yield return new WaitForSeconds(gameLogoDuration);

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(mainSceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    // --------------------------------------------------
    // MAIN MENU
    // --------------------------------------------------

    private void EnterMainMenu()
    {
        Time.timeScale = 1f;

        if (uiScreenManager != null)
        {
            uiScreenManager.ChangeScreen(ScreenType.MainMenu);
        }
    }


    private IEnumerator PrepareGame()
    {
        Time.timeScale = 1f;

        if (uiScreenManager != null)
        {
            uiScreenManager.ChangeScreen(ScreenType.Loading);
        }

        yield return new WaitForSecondsRealtime(minimumLoadingTime);

        if (LevelManager.Instance == null)
        {
            Debug.LogError(
                "GameManager: LevelManager not found."
            );

            yield break;
        }

        yield return LevelManager.Instance.LoadCurrentLevel();

        ChangeState(GameState.LevelStart);
    }

    // --------------------------------------------------
    // LEVEL START
    // --------------------------------------------------

    private IEnumerator StartLevel()
    {
        Time.timeScale = 1f;

        if (GameplayInputBinder.Instance != null)
        {
            GameplayInputBinder.Instance.ResetInput();
        }

        yield return null;
        yield return null;

        ChangeState(GameState.Playing);
    }

    // --------------------------------------------------
    // PLAYING
    // --------------------------------------------------

    private void EnterPlaying()
    {
        Time.timeScale = 1f;

        if (uiScreenManager != null)
        {
            uiScreenManager.ChangeScreen(ScreenType.GamePlay);
        }
    }

    // --------------------------------------------------
    // PAUSED
    // --------------------------------------------------

    private void EnterPaused()
    {

        if (uiScreenManager != null)
        {
            uiScreenManager.ChangeScreen(ScreenType.Pause);
        }

        Time.timeScale = 0f;
    }

    // --------------------------------------------------
    // LEVEL COMPLETE
    // --------------------------------------------------

    private void EnterLevelComplete()
    {
        Time.timeScale = 1f;

        if (uiScreenManager != null)
        {
            uiScreenManager.ChangeScreen(ScreenType.LevelComplete);
        }
    }

    // --------------------------------------------------
    // GAME OVER
    // --------------------------------------------------

    private void EnterGameOver()
    {
        Time.timeScale = 1f;

        if (uiScreenManager != null)
        {
            uiScreenManager.ChangeScreen(ScreenType.GameOver);
        }
    }

    // --------------------------------------------------
    // PUBLIC COMMANDS
    // --------------------------------------------------

    public void LoadGame()
    {
        ChangeState(GameState.Loading);
    }

    public void PauseGame()
    {
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        ChangeState(GameState.Playing);
    }

    public void CompleteLevel()
    {
        ChangeState(GameState.LevelComplete);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void ReturnToMainMenu()
    {
        if (currentState != GameState.Paused &&
            currentState != GameState.GameOver &&
            currentState != GameState.LevelComplete)
        {
            return;
        }

        stateCoroutine =
            StartCoroutine(ReturnToMainMenuRoutine());
    }

    private IEnumerator ReturnToMainMenuRoutine()
    {
        yield return LevelManager.Instance.UnloadCurrentLevel();

        ChangeState(GameState.MainMenu);
    }

    public void RetryLevel()
    {
        if (currentState == GameState.GameOver ||
            currentState == GameState.LevelComplete)
        {
            ChangeState(GameState.Loading);
        }
    }

    // --------------------------------------------------
    // HELPERS
    // --------------------------------------------------

    private void StopStateCoroutine()
    {
        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
        }
    }
}