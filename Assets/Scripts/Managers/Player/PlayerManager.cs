using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager playerAnimator;
    [HideInInspector] public PlayerMotionController playerMotionController;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerInventory playerInventory;
   
    
    [Header("Pozycja startowa")]
    public Vector3 startingPosition; // poczatkowa pozycja gracza 
    protected override void Awake()
    {
        base.Awake();

        playerMotionController = GetComponent<PlayerMotionController>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerAnimator = GetComponent<PlayerAnimatorManager>();
        playerInventory = GetComponent<PlayerInventory>();
        characterController.detectCollisions = false;
    }

    protected override void Update()
    {
        base.Update();

        // Jeśli ten obiekt nie jest przypisany do aktualnego gracza, zakończ aktualizację, aby uniknąć nieautoryzowanych zmian
        if (!IsOwner)
            return;
    
        // Obsługuje cały ruch postaci gracza
        playerMotionController.HandleAllMovementTypes();
        
        GameManager.instance.UpdatePlayerTransform(this.transform);
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            CameraController.instance.InitializeCamera(this);
            PlayerInputManager.instance.player = this;
            GameManager.instance.OnWorldSceneLoaded += HandleWorldSceneLoaded;
            InventoryUIManager.instance.InitializeAllInventories(playerInventory);
            InventoryUIManager.instance.ToggleHelpWindow();
        }
    }

    public override void OnNetworkDespawn()
    {
        
        GameManager.instance.OnWorldSceneLoaded -= HandleWorldSceneLoaded;
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;
    
        base.LateUpdate();

        // Obsługujemy w LateUpdate, aby upewnić się, że wszystkie ruchy obiektow zostały zakończone przed ustawieniem kamery
        CameraController.instance.HandleAllCameraActions();
    }
    
    private void HandleWorldSceneLoaded()
    {
        // Po tym jak uda nam się poprawnie załadować scenę, teleportuj gracza do startingPosition
        Debug.Log("loaded world!");
        GlobalInputManager.instance.isInWorldScene = true;
        StartCoroutine(TeleportPlayer(startingPosition)); // teleport
        InventoryUIManager.instance.EnablePlayerInventoryUI();
        GlobalInputManager.instance.LockPlayerCursor();
    }
    /// <summary>
/// Teleportuje gracza do określonej pozycji i rotacji, upewniając się, że świat jest w pełni załadowany, 
/// oraz dostosowuje pozycję i rotację gracza do normalnej powierzchni terenu.
/// </summary>
/// <param name="targetPosition">Docelowa pozycja, do której gracz ma zostać teleportowany.</param>
/// <param name="targetRotation">Docelowa rotacja, którą ma przyjąć gracz. Domyślnie Quaternion.identity.</param>
/// <param name="delay">Opcjonalne opóźnienie w sekundach przed wykonaniem teleportacji. Domyślnie 0.</param>
public IEnumerator TeleportPlayer(Vector3 targetPosition, Quaternion targetRotation = default, float delay = 0f)
{
    characterController.detectCollisions = false;
    
    if (delay > 0)
    {
        yield return new WaitForSeconds(delay);
    }

    // Poczekaj, aż świat będzie w pełni załadowany
    while (!GameManager.instance.worldIsSpawned)
    {
        yield return null;
    }

    // Wait for the end of the frame
    yield return null;

    bool validPositionFound = false;
    int maxAttempts = 100; // Maximum attempts to find a valid position
    Vector3 finalPosition = targetPosition;

    for (int i = 0; i < maxAttempts; i++)
    {
        // Sprawdzenie kolizji w docelowej pozycji
        if (!Physics.CheckSphere(finalPosition, characterController.radius, GameManager.instance.GetCollisionMask()))
        {
            // Wykonaj rzutowanie promienia w dół, aby dostosować rotację na podstawie normalnej powierzchni terenu
            if (Physics.Raycast(finalPosition + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, Mathf.Infinity, GameManager.instance.GetTerrainMask()))
            {
                finalPosition = hit.point + Vector3.up * characterController.height * 2f;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                validPositionFound = true;
                Debug.Log("Gracz teleportowany na pozycję: " + finalPosition + " i rotację: " + transform.rotation);
                break;
            }
        }

        // If not valid, try a new random position nearby
        finalPosition = targetPosition + Random.insideUnitSphere * 2.0f; // Adjust the multiplier as needed for range
    }

    if (!validPositionFound)
    {
        Debug.LogWarning("Nie udało się znaleźć odpowiedniej pozycji do teleportacji po " + maxAttempts + " próbach.");
    }

    // Apply the final position and enable collision detection
    transform.position = finalPosition;
    playerMotionController.canApplyGravity = true;
    characterController.detectCollisions = true;
}


}
