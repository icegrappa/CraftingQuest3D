using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stworzyłem globalny input manager dla komunikacji pomio roznych assembly 
public class GlobalInputManager : MonoBehaviour
{
    public static GlobalInputManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Movement Input")] 
    [SerializeField] public Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement Input")] 
    [SerializeField] public Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Action Input")] 
    [SerializeField] public bool interactInput;
    [SerializeField] public bool sprintInput;
    [SerializeField] public bool jumpInput;
    
    public void HandleInteractionInput(Action interactAction)
    {
        if (interactInput)
        {
            // Reset interactInput after one frame
            interactInput = false;

            // Execute the interaction action
            interactAction?.Invoke();
        }
    }
}