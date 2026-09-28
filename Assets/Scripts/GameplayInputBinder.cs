using UnityEngine;

public class GameplayInputBinder : MonoBehaviour
{
    public static GameplayInputBinder Instance { get; private set; }

    [Header("Gameplay Input")]
    [SerializeField] private FixedJoystick joystick;

    private PlayerInput currentPlayer;

    public FixedJoystick Joystick => joystick;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        TryRegisterExistingPlayer();
    }

    private void TryRegisterExistingPlayer()
    {
        PlayerInput player =
            FindAnyObjectByType<PlayerInput>();

        if (player != null)
        {
            RegisterPlayer(player);
        }
    }

    public void RegisterPlayer(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            return;
        }

        if (joystick == null)
        {
            Debug.LogError(
                "GameplayInputBinder: FixedJoystick is not assigned."
            );

            return;
        }

        currentPlayer = playerInput;

        currentPlayer.RegisterJoystick(joystick);

        Debug.Log(
            $"GameplayInputBinder: {playerInput.gameObject.name} registered."
        );
    }

    public void UnregisterPlayer(PlayerInput playerInput)
    {
        if (currentPlayer == playerInput)
        {
            currentPlayer = null;
        }
    }

    public void ResetInput()
    {
        if (joystick != null)
        {
            joystick.ResetJoystick();
        }
    }
}