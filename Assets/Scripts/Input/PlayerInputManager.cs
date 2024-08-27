using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    private PlayerControls _playerControls;
    public PlayerManager player;

    [Header("Movement Input")] [SerializeField]
    private Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement Input")] [SerializeField]
    private Vector2 cameraInput;

    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Action Input")] [SerializeField]
    private bool interact;

    [SerializeField] private bool sprintInput;
    [SerializeField] private bool jumpInput;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject); // 

        // Subskrybuje event, który wywołuje metodę OnSceneChange za każdym razem, gdy zmieniamy scenę
        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false; // Wyłączenie instancji na starcie; będzie włączona tylko w określonej scenie

        if (_playerControls != null) _playerControls.Disable();
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // Sprawdza, czy nowo załadowana scena to główna scena świata; jeśli tak, aktywuje instancję
        if (newScene.buildIndex == GameManager.instance.GetWorldSceneIndex())
            instance.enabled = true; // Włączenie instancji w głównej scenie świata
        if (_playerControls != null)
            _playerControls.Enable();
        else
            instance.enabled = false; // Wyłączenie instancji, jeśli nie jesteśmy w głównej scenie świata
        if (_playerControls != null) _playerControls.Disable();
    }


    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();

            _playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerControls.CameraMovement.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();

            // shift + hold decyduje o fladze dla sprintu
            _playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            _playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
            //Jump input
            _playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
        }

        _playerControls.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    // Jeśli zminimalizujemy lub opuścimy okno aplikacji, zatrzymaj przetwarzanie inputu
    private void OnApplicationFocus(bool hasFocus)
    {
        if (enabled)
        {
            if (hasFocus)
                _playerControls.Enable();
            else
                _playerControls.Disable();
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleSprintInput();
        HandleJumpInput();
    }

    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        // Clampuje wartość moveAmount między 0 a 1, sumując absolutne wartości inputu. 
        // Zapobiega to przekroczeniu 1, co mogłoby powodować nieprzewidziane zachowanie.
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        // Przeskalowujemy moveAmount do 0.5 lub 1, aby zapewnić bardziej responsywne sterowanie.
        // Dzięki temu minimalny ruch jest bardziej odczuwalny, a pełna prędkość osiągana jest przy większym wejściu.
        if (moveAmount <= 0.5 && moveAmount > 0)
            moveAmount = 0.5f;
        else if (moveAmount >= 0.5 && moveAmount <= 1) moveAmount = 1;

        player.playerAnimator.UpdateAnimatorMovementValues(0, moveAmount,
            player.playerNetworkManager.isSprinting.Value);
    }

    private void HandleCameraMovementInput()
    {
        // Obsługa wejścia z myszy/PADA
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
            player.playerMotionController.HandleSprinting();
        else
            player.playerNetworkManager.isSprinting.Value = false;
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            player.playerMotionController.HandleJumping();
        }
    }
}