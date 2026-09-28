using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("Keyboard / Gamepad")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private PlayerMovement playerMovement;
    private FixedJoystick joystick;

    private Vector2 moveInput;

    public Vector2 MoveInput => moveInput;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        jumpAction?.action.Enable();
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
        jumpAction?.action.Disable();
    }

    private void Start()
    {
        RegisterWithGameplayUI();
    }

    private void RegisterWithGameplayUI()
    {
        if (GameplayInputBinder.Instance == null)
        {
            Debug.LogWarning(
                "PlayerInput: GameplayInputBinder not ready."
            );

            return;
        }

        GameplayInputBinder.Instance.RegisterPlayer(this);
    }

    private void Update()
    {
        ReadMoveInput();

        if (jumpAction != null &&
            jumpAction.action.WasPerformedThisFrame())
        {
            playerMovement.Jump();
        }
    }

    private void ReadMoveInput()
    {
        Vector2 actionInput = moveAction != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;

        if (joystick != null &&
            joystick.Input.sqrMagnitude > 0.01f)
        {
            moveInput = joystick.Input;
        }
        else
        {
            moveInput = actionInput;
        }
    }

    public void RegisterJoystick(FixedJoystick newJoystick)
    {
        joystick = newJoystick;

        Debug.Log(
            $"PlayerInput: Joystick registered = {joystick.gameObject.name}"
        );
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void JumpButton()
    {
        playerMovement.Jump();
    }

    private void OnDestroy()
    {
        if (GameplayInputBinder.Instance != null)
        {
            GameplayInputBinder.Instance.UnregisterPlayer(this);
        }
    }
}