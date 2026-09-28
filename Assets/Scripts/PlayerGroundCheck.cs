using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        Vector3 origin =
            transform.position + Vector3.up * 0.1f;

        IsGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            groundCheckDistance + 0.1f,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 origin =
            transform.position + Vector3.up * 0.1f;

        Gizmos.DrawLine(
            origin,
            origin + Vector3.down *
            (groundCheckDistance + 0.1f)
        );
    }
}