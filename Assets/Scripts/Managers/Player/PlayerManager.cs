using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager playerAnimator;
    [HideInInspector] public PlayerMotionController playerMotionController;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    
    [Header("Pozycja startowa")]
    public Vector3 startingPosition; // poczatkowa pozycja gracza 
    protected override void Awake()
    {
        base.Awake();

        playerMotionController = GetComponent<PlayerMotionController>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerAnimator = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        // Jeśli ten obiekt nie jest przypisany do aktualnego gracza, zakończ aktualizację, aby uniknąć nieautoryzowanych zmian
        if (!IsOwner)
            return;
    
        // Obsługuje cały ruch postaci gracza
        playerMotionController.HandleAllMovementTypes();
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            CameraController.instance.InitializeCamera(this);
            PlayerInputManager.instance.player = this;
            GameManager.instance.OnWorldSceneLoaded += HandleWorldSceneLoaded;
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
        StartCoroutine(TeleportPlayer(startingPosition)); // teleport
    }
    
    /// <summary>
    /// Teleportuje gracza do określonej pozycji i rotacji, z opcjonalnym opóźnieniem.
    /// </summary>
    /// <param name="targetPosition">Docelowa pozycja, do której gracz ma zostać teleportowany.</param>
    /// <param name="targetRotation">Docelowa rotacja, którą ma przyjąć gracz. Domyślnie Quaternion.identity.</param>
    /// <param name="delay">Opcjonalne opóźnienie w sekundach przed wykonaniem teleportacji. Domyślnie 0.</param>
    public IEnumerator TeleportPlayer(Vector3 targetPosition, Quaternion targetRotation = default, float delay = 0f)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
    
        // Teleportacja gracza do zadanej pozycji i rotacji
        transform.position = targetPosition;
        transform.rotation = targetRotation == default ? Quaternion.identity : targetRotation;
    
        Debug.Log("Gracz teleportowany na pozycję: " + targetPosition + " i rotację: " + targetRotation);
        
        playerMotionController.canApplyGravity = true;
    }


}
