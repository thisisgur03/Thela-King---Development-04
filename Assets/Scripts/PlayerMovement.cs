/*
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpForce;

    [Header("Input")]
    [SerializeField] private InputActionReference jumpAction;

    [Header("Fixed Joystick")]
    [SerializeField] private RectTransform joystick;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private float joystickRadius = 100f;

    private Rigidbody rigidbody;

    private Vector2 moveInput;
    private Vector2 joystickInput;

    private bool isGrounded;
    private bool joystickActive;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        jumpAction.action.Disable();
    }

    private void Update()
    {
        HandleJoystick();

        if (joystickActive)
            moveInput = joystickInput;
        else
            moveInput = Vector3.zero;

        Jump();
        
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();

        rigidbody.angularVelocity = Vector3.zero;
    }

    private void HandleJoystick()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // Press
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 joystickCenter = joystick.position;

            // Only activate if clicking inside joystick area
            float distance = Vector2.Distance(mousePosition, joystickCenter);

            if (distance <= joystickRadius)
            {
                joystickActive = true;
                joystickInput = Vector2.zero;
            }
        }

        // Hold
        if (joystickActive && Mouse.current.leftButton.isPressed)
        {
            Vector2 joystickCenter = joystick.position;

            Vector2 offset = mousePosition - joystickCenter;

            offset = Vector2.ClampMagnitude(offset, joystickRadius);

            joystickHandle.position = joystickCenter + offset;

            joystickInput = offset / joystickRadius;
        }

        // Release
        if (joystickActive && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            joystickActive = false;
            joystickInput = Vector2.zero;

            // Return handle to center
            joystickHandle.position = joystick.position;
        }
    }

    private void Move()
    {
        Vector3 direction = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        Vector3 velocity = rigidbody.linearVelocity;

        velocity.x = direction.x * moveSpeed;
        velocity.z = direction.z * moveSpeed;

        rigidbody.linearVelocity = velocity;
    }

    private void Jump()
    {
        if (jumpAction.action.WasPerformedThisFrame() && isGrounded)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Rotate()
    {
        Vector3 direction = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        rigidbody.MoveRotation(
            Quaternion.Slerp(
                rigidbody.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
*/
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody body;
    private PlayerInput playerInput;
    private PlayerGroundCheck groundCheck;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        groundCheck = GetComponent<PlayerGroundCheck>();
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();

        body.angularVelocity = Vector3.zero;
    }

    private void Move()
    {
        Vector2 input = playerInput.MoveInput;

        Vector3 direction = new Vector3(
            input.x,
            0f,
            input.y
        );

        Vector3 velocity = body.linearVelocity;

        velocity.x = direction.x * moveSpeed;
        velocity.z = direction.z * moveSpeed;

        body.linearVelocity = velocity;
    }

    private void Rotate()
    {
        Vector2 input = playerInput.MoveInput;

        Vector3 direction = new Vector3(
            input.x,
            0f,
            input.y
        );

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        Quaternion newRotation =
            Quaternion.Slerp(
                body.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        body.MoveRotation(newRotation);
    }

    public void Jump()
    {
        if (!groundCheck.IsGrounded)
        {
            return;
        }

        body.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }
}