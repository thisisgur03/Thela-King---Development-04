using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Text levelText;

    private void Start()
    {
        if (LevelManager.Instance == null)
        {
            return;
        }

        LevelManager.Instance.OnLevelChanged += UpdateLevelText;

        UpdateLevelText(
            LevelManager.Instance.CurrentLevelIndex
        );
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelChanged -= UpdateLevelText;
        }
    }

    private void UpdateLevelText(int levelIndex)
    {
        levelText.text =
            $"Level {levelIndex + 1}";
    }
}