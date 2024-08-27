using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public PlayerManager player;
    public Camera cameraObject;

    [SerializeField] private Transform cameraPivot;

    [Header("Ustawienia kamery")] 
    [SerializeField] private float cameraSmoothSpeed = 1f; // jak szybko kamera lagodnie sie przemieszcza
    [SerializeField] private float leftAndRightRotationSpeed = 220f; // predkosc obrotu kamery lewo/prawo
    [SerializeField] private float upAndDownRotationSpeed = 220f; // predkosc obrotu kamery gora/dol
    [SerializeField] private float minimumPivot = -30f; // minimalny kat w pionie
    [SerializeField] private float maximumPivot = 60f; // maksymalny kat w pionie

    [SerializeField] private float cameraCollisionRadius = 0.2f; // promien wykrywania kolizji kamery
    [SerializeField] LayerMask collideWithLayers; // warstwy z ktorymi koliduje kamera


    [Header("Wartości kamery")] 
    [SerializeField] private Vector3 cameraVelocity; // predkosc kamery dla smooth ruchu
    [SerializeField] private Vector3 cameraObjectPosition; // pozycja kamery po kolizji
    [SerializeField] private float leftAndRightLookAngle; // kat kamery lewo/prawo
    [SerializeField] private float upAndDownLookAngle; // kat kamery gora/dol
    [SerializeField]private float cameraZPosition; // obecna pozycja Z kamery
    [SerializeField]private float targetCameraZPosition; // docelowa pozycja Z kamery po kolizji

    
    private CameraPositionController positionController;
    private CameraRotationController rotationController;
    private CameraCollisionController collisionController;
    private bool _initialized;
   
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        cameraObject = GetComponentInChildren<Camera>();
        
    }
    
    public void InitializeCamera(PlayerManager player)
    {
        this.player = player;

        // Inicjalizacja kontrolerów
        positionController = new CameraPositionController(transform, player.transform, cameraSmoothSpeed);
        rotationController = new CameraRotationController(transform, cameraPivot, leftAndRightRotationSpeed, upAndDownRotationSpeed, minimumPivot, maximumPivot);
        collisionController = new CameraCollisionController(cameraObject.transform, cameraPivot, cameraCollisionRadius, collideWithLayers);
        
        _initialized = true;
    }


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }
    
    private void Update()
    {
        if (!_initialized) return;
        // worc jezeli nie otrzymalismy managera gracza

        positionController.UpdatePositionSettings(cameraSmoothSpeed);
        rotationController.UpdateRotationSettings(leftAndRightRotationSpeed, upAndDownRotationSpeed, minimumPivot, maximumPivot);
        collisionController.UpdateCollisionSettings(cameraCollisionRadius, collideWithLayers);
        cameraVelocity = positionController.CameraVelocity;
        leftAndRightLookAngle = rotationController.LeftAndRightLookAngle;
        upAndDownLookAngle = rotationController.UpAndDownLookAngle;
        cameraZPosition = collisionController.CameraZPosition;
        targetCameraZPosition = collisionController.TargetCameraZPosition;
    }

    public void HandleAllCameraActions()
    {
        //POZYCJA
        positionController.UpdateCameraPosition();
        //ROTACJA
        rotationController.UpdateCameraRotation();
        //KOLIZJE
        collisionController.HandleCollisions();
    }
    
}