using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    private PlayerControls _playerControls;
    public PlayerManager player;

     [Header("Movement Input")] 
    [SerializeField] private Vector2 movementInput;

   public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement Input")] 
    [SerializeField] private Vector2 cameraInput;

    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Action Input")] [SerializeField]
    private bool interact = false;


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
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // Sprawdza, czy nowo załadowana scena to główna scena świata; jeśli tak, aktywuje instancję
        if (newScene.buildIndex == GameManager.instance.GetWorldSceneIndex())
            instance.enabled = true; // Włączenie instancji w głównej scenie świata
        else
            instance.enabled = false; // Wyłączenie instancji, jeśli nie jesteśmy w głównej scenie świata
    }


    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();

            _playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerControls.CameraMovement.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
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
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
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
        
        player.playerAnimator.UpdateAnimatorMovementValues(0 , moveAmount);
    }

    private void HandleCameraMovementInput()
    {
        // Obsługa wejścia z myszy/PADA
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }
}