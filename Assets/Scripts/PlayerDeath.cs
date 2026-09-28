using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float deathHeight = -10f;

    private bool isDead;

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (transform.position.y <= deathHeight)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        GameManager.Instance.GameOver();
    }
}