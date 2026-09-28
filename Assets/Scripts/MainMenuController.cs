using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIScreenManager uiScreenManager;

    public void Play()
    {
        LevelManager.Instance.StartLevel(LevelManager.Instance.HighestUnlockedLevel);
    }

    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void MainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void Pause()
    {
        GameManager.Instance.PauseGame();
    }

    public void LevelComplete()
    {
        LevelManager.Instance.CompleteCurrentLevel();
    }

    public void GameOver()
    {
        GameManager.Instance.GameOver();
    }

    public void Retry()
    {
        LevelManager.Instance.RestartLevel();
    }

    public void NextLevel()
    {
        LevelManager.Instance.LoadNextLevel();
    }


    public void OpenSettings()
    {
        uiScreenManager.ChangeScreen(ScreenType.Settings);
    }

    public void CloseSettings()
    {
        uiScreenManager.ReturnToPreviousScreen();
    }
}