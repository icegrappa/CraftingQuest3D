using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public  CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public  CharacterNetworkManager characterNetworkManager;

    [Header("Flags")] public bool isPerformingAction = false;
    public bool isGrounded = true;
    public bool applyRootMotion = false;
    public bool canRotate = false;
    public bool canMove = false;
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        
        characterController = GetComponent<CharacterController>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        animator.SetBool(characterAnimatorManager._isGrounded, isGrounded);
        // Jeśli ta postać jest kontrolowana z naszej strony, przypisujemy jej sieciową pozycję i rotację naszemu transformowi
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        
        // Jeśli ta postać jest kontrolowana z innej strony, przypisujemy jej lokalną pozycję na podstawie sieciowej pozycji
        else
        {
            //Pozycja
            transform.position = Vector3.SmoothDamp(transform.position, 
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity, 
                characterNetworkManager.networkPositionSmoothTime);
            
            //Rotacja
            transform.rotation = Quaternion.Slerp
                (transform.rotation, 
                    characterNetworkManager.networkRotation.Value, 
                    characterNetworkManager.networkRotationSmoothTime);
        }
    }

    protected virtual void LateUpdate()
    {
        
    }
}
