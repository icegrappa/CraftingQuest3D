using UnityEngine;

public class PlayerMotionController : CharacterMotionController
{
    private PlayerManager player;

    [HideInInspector]public float verticalMovement;
    [HideInInspector]public float horizontalMovement;
     [HideInInspector]public float moveAmount;

    
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [Header("Ustawienia Ruchu")]
    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 5;
    [SerializeField] private float sprintSpeed = 8;
    [Header("Ustawienia Skoku")]
    [SerializeField] float jumpHeight = 3;
    [SerializeField] float jumpForwardSpeed = 4;
    [SerializeField] float freeFallSpeed = 1;
    private Vector3 jumpDirection;
    [Header("Ustawienia Rotacji")]
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
            
            player.playerAnimator.UpdateAnimatorMovementValues(0 , moveAmount, player.playerNetworkManager.isSprinting.Value);
        }
    }

    public void HandleAllMovementTypes()
    {
        HandleGroundedMovement();
        HandleRotation();
        
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    private void UpdateMovementsValues()
    {
        verticalMovement = GlobalInputManager.instance.verticalInput;
        horizontalMovement = GlobalInputManager.instance.horizontalInput;
        moveAmount = GlobalInputManager.instance.moveAmount;
    }

    private void HandleGroundedMovement()
    {
        //
        if (!player.canMove) return;
        
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

        // Obliczamy kierunek ruchu na podstawie kamery i wejścia
        moveDirection = CameraController.instance.transform.forward * verticalMovement;
        moveDirection += CameraController.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // oblicz predkosc
        float speed = CalculateMovementSpeed();
        
        // Przemieszczamy postać z użyciem obliczonego kierunku i prędkości
        player.characterController.Move(moveDirection * (speed * Time.deltaTime));

    }

    private void HandleJumpingMovement()
    {
        // Sprawdzenie, czy gracz jest w trakcie skoku
        if (!player.playerNetworkManager.isJumping.Value) return;

        // Przemieszczanie gracza w kierunku skoku z zastosowaniem prędkości skoku do przodu
        Vector3 jumpMovement = jumpDirection * (jumpForwardSpeed * Time.deltaTime);

        player.characterController.Move(jumpMovement);
    }

    private void HandleFreeFallMovement()
    {
        // Sprawdzenie, czy gracz jest w stanie swobodnego spadania (czyli nie jest na ziemi)
        if (player.isGrounded) return;

        // Obliczenie kierunku swobodnego spadania na podstawie kierunku kamery i wejścia gracza
        Vector3 freeFallDirection = CalculateFallMovementDirection();

        // Przemieszczanie gracza w kierunku swobodnego spadania z zastosowaniem prędkości swobodnego spadania
        Vector3 freeFallMovement = freeFallDirection * (freeFallSpeed * Time.deltaTime);

        player.characterController.Move(freeFallMovement);
    }


    private Vector3 CalculateFallMovementDirection()
    {
        // Obliczenie kierunku na podstawie kierunku kamery i wejścia gracza
        Vector3 direction = CameraController.instance.transform.forward * GlobalInputManager.instance.verticalInput;
        direction += CameraController.instance.transform.right * GlobalInputManager.instance.horizontalInput;
        direction.y = 0; // Ignorujemy komponent Y, ponieważ chcemy się poruszać tylko na płaszczyźnie XZ
        return direction;
    }


    /// <summary>
    /// Oblicza prędkość na podstawie aktualnego stanu gracza (chód, bieg, sprint).
    /// </summary>
    /// <returns>Prędkość postaci.</returns>
    private float CalculateMovementSpeed()
    {
        if (player.playerNetworkManager.isSprinting.Value)
        {
            return sprintSpeed;
        }

        return GlobalInputManager.instance.moveAmount > 0.5f ? runningSpeed : walkingSpeed;
    }

    private void HandleRotation()
    {
        //ROTACJA
        if (!player.canRotate) return;
        
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

    public void HandleSprinting()
    {
        if (player.isPerformingAction) player.playerNetworkManager.isSprinting.Value = false;

        if (moveAmount >= 0.5)
            player.playerNetworkManager.isSprinting.Value = true;
        else
            player.playerNetworkManager.isSprinting.Value = false;
    }

    public void HandleJumping()
    {
        if (player.isPerformingAction)
            return;

        if (player.playerNetworkManager.isJumping.Value)
            return;

        if (!player.isGrounded)
            return;

        //animacja skokuuy
        player.playerAnimator.PlayActionAnimation("Jump_Start", false);

        player.playerNetworkManager.isJumping.Value = true;

        this.jumpDirection = CalculateJumpDirection();
    }
    
    
    private Vector3 CalculateJumpDirection()
    {
        // Pobierz kierunek skoku na podstawie kierunku kamery i wejścia gracza
        Vector3 calculatedJumpDirection = CameraController.instance.cameraObject.transform.forward * 
                                          GlobalInputManager.instance.verticalInput;
        calculatedJumpDirection += CameraController.instance.cameraObject.transform.right * GlobalInputManager.instance.horizontalInput;
        calculatedJumpDirection.y = 0;

        if (calculatedJumpDirection != Vector3.zero)
        {
            // Modyfikuj kierunek skoku w zależności od tego, czy gracz sprintuje, czy nie
            if (player.playerNetworkManager.isSprinting.Value)
            {
                calculatedJumpDirection *= 1;
            }
            else if (GlobalInputManager.instance.moveAmount > 0.5)
            {
                calculatedJumpDirection *= 0.5f;
            }
            else
            {
                calculatedJumpDirection *= 0.25f;
            }

        }
        
        return calculatedJumpDirection;
    }



    //Event który calluje w animacji Jump_Start
    public void ApplyJumpForce()
    {
        // Oblicza siłę skoku, ustawiając prędkość Y dla wysokosci skoku 
        //https://discussions.unity.com/t/how-to-calculate-force-needed-to-jump-towards-target-point/607902/12
        yVelocity.y = Mathf.Sqrt((jumpHeight * -2 * gravityForce));
    }

}