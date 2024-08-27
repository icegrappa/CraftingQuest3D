using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager playerAnimator;
    [HideInInspector] public PlayerMotionController playerMotionController;
    protected override void Awake()
    {
        base.Awake();

        playerMotionController = GetComponent<PlayerMotionController>();
        playerAnimator = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        // Jeśli ten obiekt nie jest przypisany do aktualnego gracza, zakończ aktualizację, aby uniknąć nieautoryzowanych zmian
        if (!IsOwner)
            return;
    
        // Obsługuje cały ruch postaci gracza
        playerMotionController.HandleAllMovement();
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            CameraController.instance.InitializeCamera(this);
            PlayerInputManager.instance.player = this;
        }
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;
    
        base.LateUpdate();

        // Obsługujemy w LateUpdate, aby upewnić się, że wszystkie ruchy obiektow zostały zakończone przed ustawieniem kamery
        CameraController.instance.HandleAllCameraActions();
    }

}
