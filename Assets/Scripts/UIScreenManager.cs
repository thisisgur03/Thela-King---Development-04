/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScreenType
{
    CompanyLogo,
    GameLogo,
    MainMenu,
    Loading,
    GamePlay,
    Settings,
    Pause,
    Confirmation,
    GameOver,
    LevelComplete
}
public class UIScreenManager : MonoBehaviour
{
    public static UIScreenManager Instance { get; private set; }
    [SerializeField] private Dictionary<ScreenType, GameObject> UIScreens = new Dictionary<ScreenType, GameObject>();

    private ScreenType currentScreen;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentScreen = ScreenType.CompanyLogo;
        ShowUI(currentScreen);
    }

    private bool IsValidTransition(ScreenType newScreen)
    {
        switch(currentScreen)
        {
            case ScreenType.CompanyLogo:
                if(newScreen == ScreenType.GameLogo)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.GameLogo:
                if (newScreen == ScreenType.MainMenu || newScreen == ScreenType.Loading)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.MainMenu:
                if (newScreen == ScreenType.Loading || newScreen == ScreenType.Settings || newScreen == ScreenType.Confirmation)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.Loading:
                if (newScreen == ScreenType.GamePlay || newScreen == ScreenType.MainMenu)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.GamePlay:
                if (newScreen == ScreenType.Pause || newScreen == ScreenType.Settings || newScreen == ScreenType.GameOver || newScreen == ScreenType.LevelComplete)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.Settings:
                if (newScreen == ScreenType.MainMenu || newScreen == ScreenType.Pause || newScreen == ScreenType.Confirmation)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.Pause:
                if (newScreen == ScreenType.GamePlay || newScreen == ScreenType.Settings || newScreen == ScreenType.Confirmation || newScreen == ScreenType.MainMenu)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.Confirmation:
                if (newScreen == ScreenType.MainMenu || newScreen == ScreenType.Settings || newScreen == ScreenType.Pause || newScreen == ScreenType.GameOver)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.GameOver:
                if (newScreen == ScreenType.Loading || newScreen == ScreenType.MainMenu || newScreen == ScreenType.Confirmation)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ScreenType.LevelComplete:
                if (newScreen == ScreenType.Loading || newScreen == ScreenType.MainMenu || newScreen == ScreenType.GamePlay)
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

    public void ChangeScreen(ScreenType newScreen)
    {
        if(IsValidTransition(newScreen))
        {
            HideUI(currentScreen);
            ShowUI(newScreen);

            currentScreen = newScreen;
        }
        else
        {
            Debug.Log("Not a Valid UI Transition");
        }
    }

    private void ShowUI(ScreenType screenType)
    {
        UIScreens[screenType].SetActive(true);
    }

    private void HideUI(ScreenType screenType)
    {
        UIScreens[screenType].SetActive(false);
    }
}
*/

using System.Collections.Generic;
using UnityEngine;

public enum ScreenType
{
    CompanyLogo,
    GameLogo,
    MainMenu,
    Loading,
    GamePlay,
    Settings,
    Pause,
    Confirmation,
    GameOver,
    LevelComplete
}

[System.Serializable]
public class ScreenEntry
{
    public ScreenType screenType;
    public GameObject screenObject;
}

public class UIScreenManager : MonoBehaviour
{
    [Header("Initial Screen")]
    [SerializeField] private ScreenType initialScreen;

    [Header("Screens")]
    [SerializeField] private List<ScreenEntry> screens = new List<ScreenEntry>();

    private Dictionary<ScreenType, GameObject> screenDictionary;

    private ScreenType currentScreen;
    private ScreenType previousScreen;

    public ScreenType CurrentScreen => currentScreen;

    private void Awake()
    {
        BuildDictionary();
        HideAllScreens();

    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterUIScreenManager(this);
        }

        ShowInitialScreen();
    }


    // --------------------------------------------------
    // INITIALIZATION
    // --------------------------------------------------

    private void BuildDictionary()
    {
        screenDictionary = new Dictionary<ScreenType, GameObject>();

        foreach (ScreenEntry entry in screens)
        {
            if (entry == null)
            {
                continue;
            }

            if (entry.screenObject == null)
            {
                Debug.LogWarning(
                    $"UIScreenManager: {entry.screenType} has no GameObject assigned."
                );

                continue;
            }

            if (screenDictionary.ContainsKey(entry.screenType))
            {
                Debug.LogWarning(
                    $"UIScreenManager: Duplicate screen type {entry.screenType}."
                );

                continue;
            }

            screenDictionary.Add(
                entry.screenType,
                entry.screenObject
            );
        }
    }

    private void ShowInitialScreen()
    {
        if (!screenDictionary.ContainsKey(initialScreen))
        {
            Debug.LogError(
                $"UIScreenManager: Initial screen {initialScreen} is not registered."
            );

            return;
        }

        currentScreen = initialScreen;

        ShowUI(currentScreen);
    }

    // --------------------------------------------------
    // SCREEN TRANSITION
    // --------------------------------------------------

    public void ChangeScreen(ScreenType newScreen)
    {
        if (currentScreen == newScreen)
        {
            return;
        }

        if (!screenDictionary.ContainsKey(newScreen))
        {
            Debug.LogWarning(
                $"UIScreenManager: Screen {newScreen} is not registered."
            );

            return;
        }

        previousScreen = currentScreen;

        HideUI(currentScreen);

        currentScreen = newScreen;

        ShowUI(currentScreen);
    }

    public void ReturnToPreviousScreen()
    {
        ScreenType targetScreen = previousScreen;

        HideUI(currentScreen);

        currentScreen = targetScreen;

        ShowUI(currentScreen);
    }

    // --------------------------------------------------
    // SHOW / HIDE
    // --------------------------------------------------

    private void ShowUI(ScreenType screenType)
    {
        if (!screenDictionary.TryGetValue(
                screenType,
                out GameObject screen))
        {
            Debug.LogWarning(
                $"UIScreenManager: Cannot find {screenType}."
            );

            return;
        }

        screen.SetActive(true);
    }

    private void HideUI(ScreenType screenType)
    {
        if (!screenDictionary.TryGetValue(
                screenType,
                out GameObject screen))
        {
            return;
        }

        screen.SetActive(false);
    }

    private void HideAllScreens()
    {
        foreach (GameObject screen in screenDictionary.Values)
        {
            if (screen != null)
            {
                screen.SetActive(false);
            }
        }
    }

    // --------------------------------------------------
    // PUBLIC HELPERS
    // --------------------------------------------------

    public bool IsCurrentScreen(ScreenType screenType)
    {
        return currentScreen == screenType;
    }
}