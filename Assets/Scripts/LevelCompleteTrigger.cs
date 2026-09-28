using UnityEngine;

public class LevelCompleteTrigger : MonoBehaviour
{
    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if (completed)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        completed = true;

        LevelManager.Instance.CompleteCurrentLevel();
    }
}