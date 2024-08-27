using UnityEngine;

public class PlayerMotionController : CharacterMotionController
{
    private PlayerManager player;

    [HideInInspector]public float verticalMovement;
    [HideInInspector]public float horizontalMovement;
     [HideInInspector]public float moveAmount;

     [Header("Movement Settings")] 
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 5;
    [SerializeField] private float rotationSpeed = 15;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetworkManager.verticalNetMovement.Value = verticalMovement;
            player.characterNetworkManager.horizontalNetMovement.Value = verticalMovement;
            player.characterNetworkManager.moveNetAmount.Value = moveAmount;
        }
        else
        {
            verticalMovement = player.characterNetworkManager.verticalNetMovement.Value;
            horizontalMovement = player.characterNetworkManager.horizontalNetMovement.Value;
            moveAmount = player.characterNetworkManager.moveNetAmount.Value;
        }
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
    }

    private void UpdateMovementsValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
        //Clamp The Movements
    }

    private void HandleGroundedMovement()
    {
        if (player == null || player.characterController == null)
        {
            Debug.LogError("Player or CharacterController is not initialized.");
            return;
        }

        if (PlayerInputManager.instance == null)
        {
            Debug.LogError("PlayerInputManager.instance is null.");
            return;
        }

        if (CameraController.instance == null)
        {
            Debug.LogError("CameraController.instance is null.");
            return;
        }

        UpdateMovementsValues();

        // Nasz kierunek ruchu opiera się na perspektywie kamery oraz wejściach ruchu
        moveDirection = CameraController.instance.transform.forward * verticalMovement;
        moveDirection += CameraController.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // Obliczamy jednorazowy mnożnik prędkości
        var speed = PlayerInputManager.instance.moveAmount > 0.5f ? runningSpeed : walkingSpeed;
        speed *= Time.deltaTime;

        // Ruch postaci z użyciem zoptymalizowanego mnożnika
        player.characterController.Move(moveDirection * speed);
    }

    private void HandleRotation()
    {
        // Obliczenie kierunku docelowego na podstawie kierunku kamery i wejść ruchu w jednej linii
        targetRotationDirection = CameraController.instance.cameraObject.transform.forward * verticalMovement
                                  + CameraController.instance.cameraObject.transform.right * horizontalMovement;

        // Zerowanie składowej Y, aby obrót był w płaszczyźnie XZ
        targetRotationDirection.y = 0;

        // Normalizacja wektora, aby zapewnić jego jednostkową długość
        targetRotationDirection.Normalize();

        // Sprawdzenie, czy kierunek jest znaczący przed obrotem
        if (targetRotationDirection.sqrMagnitude > 0.01f)
        {
            var newTargetRotation = Quaternion.LookRotation(targetRotationDirection);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, newTargetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}