using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelDefinition
{
    public int levelIndex;
    public string levelName;
    public string sceneName;
}

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private List<LevelDefinition> levels = new();

    public static LevelManager Instance { get; private set; }

    private int currentLevelIndex = -1;
    private Scene currentLevelScene;

    public int CurrentLevelIndex => currentLevelIndex;

    private const string HighestUnlockedLevelKey = "HighestUnlockedLevel";

    private int highestUnlockedLevel;

    public int HighestUnlockedLevel => highestUnlockedLevel;

    public event System.Action<int> OnLevelChanged;

    public LevelDefinition CurrentLevel
    {
        get
        {
            if (currentLevelIndex < 0 ||
                currentLevelIndex >= levels.Count)
            {
                return null;
            }

            return levels[currentLevelIndex];
        }
    }

    public bool HasLoadedLevel =>
        currentLevelScene.IsValid() &&
        currentLevelScene.isLoaded;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        highestUnlockedLevel = PlayerPrefs.GetInt(HighestUnlockedLevelKey, 0);
    }


    // --------------------------------------------------
    // LEVEL SELECTION
    // --------------------------------------------------

    public void StartLevel(int levelIndex)
    {
        if (!IsValidLevel(levelIndex))
        {
            Debug.LogError(
                $"LevelManager: Invalid level index {levelIndex}."
            );

            return;
        }

        if (levelIndex > highestUnlockedLevel)
        {
            Debug.LogWarning(
                $"Level {levelIndex} is locked."
            );

            return;
        }

        currentLevelIndex = levelIndex;
        OnLevelChanged?.Invoke(currentLevelIndex);

        GameManager.Instance.LoadGame();
    }


    public void RestartLevel()
    {
        if (CurrentLevel == null)
        {
            Debug.LogError(
                "LevelManager: No current level selected."
            );

            return;
        }

        GameManager.Instance.RetryLevel();
    }


    public void LoadNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        if (!IsValidLevel(nextLevelIndex))
        {
            Debug.Log(
                "LevelManager: No more levels."
            );

            return;
        }

        currentLevelIndex = nextLevelIndex;
        OnLevelChanged?.Invoke(currentLevelIndex);

        GameManager.Instance.LoadGame();
    }


    // --------------------------------------------------
    // LEVEL LOADING
    // --------------------------------------------------

    public IEnumerator LoadCurrentLevel()
    {
        if (CurrentLevel == null)
        {
            Debug.LogError(
                "LevelManager: No current level selected."
            );

            yield break;
        }

        // Unload previous level first.
        yield return UnloadCurrentLevel();

        string sceneName = CurrentLevel.sceneName;

        Debug.Log(
            $"LevelManager: Loading {sceneName}"
        );

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                sceneName,
                LoadSceneMode.Additive
            );

        if (operation == null)
        {
            Debug.LogError(
                $"LevelManager: Failed to load {sceneName}."
            );

            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        currentLevelScene =
            SceneManager.GetSceneByName(sceneName);

        if (!currentLevelScene.IsValid())
        {
            Debug.LogError(
                $"LevelManager: Loaded scene could not be found: {sceneName}"
            );

            yield break;
        }

        Debug.Log(
            $"LevelManager: {sceneName} loaded."
        );
    }


    // --------------------------------------------------
    // LEVEL UNLOADING
    // --------------------------------------------------

    public IEnumerator UnloadCurrentLevel()
    {
        if (!HasLoadedLevel)
        {
            currentLevelScene = default;
            yield break;
        }

        string sceneName = currentLevelScene.name;

        Debug.Log(
            $"LevelManager: Unloading {sceneName}"
        );

        AsyncOperation operation =
            SceneManager.UnloadSceneAsync(
                currentLevelScene
            );

        if (operation == null)
        {
            Debug.LogWarning(
                $"LevelManager: Could not unload {sceneName}."
            );

            currentLevelScene = default;
            yield break;
        }

        while (!operation.isDone)
        {
            yield return null;
        }

        currentLevelScene = default;

        Debug.Log(
            $"LevelManager: {sceneName} unloaded."
        );
    }


    // --------------------------------------------------
    // LEVEL STATE
    // --------------------------------------------------

    public void CompleteCurrentLevel()
    {
        UnlockNextLevel();

        GameManager.Instance.CompleteLevel();
    }

    private void UnlockNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;

        if (!IsValidLevel(nextLevelIndex))
        {
            return;
        }

        if (nextLevelIndex <= highestUnlockedLevel)
        {
            return;
        }

        highestUnlockedLevel = nextLevelIndex;

        PlayerPrefs.SetInt(
            HighestUnlockedLevelKey,
            highestUnlockedLevel
        );

        PlayerPrefs.Save();

        Debug.Log(
            $"Level {nextLevelIndex} unlocked."
        );
    }


    public bool HasNextLevel()
    {
        return IsValidLevel(currentLevelIndex + 1);
    }


    // --------------------------------------------------
    // VALIDATION
    // --------------------------------------------------

    private bool IsValidLevel(int levelIndex)
    {
        return levelIndex >= 0 &&
               levelIndex < levels.Count;
    }
}