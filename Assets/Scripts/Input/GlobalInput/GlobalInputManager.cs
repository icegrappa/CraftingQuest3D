using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stworzyłem globalny input manager dla komunikacji pomio roznych assembly 
public class GlobalInputManager : MonoBehaviour
{
    public static GlobalInputManager instance;
    
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
    
    [Header("Inventory Input")] 
    [SerializeField] public bool inventoryInput;
    [SerializeField] public bool stackInput;
    [SerializeField] public bool deleteItemUIInput;
    [SerializeField] public bool isInventoryOpen = false;

    [Header("Scene flag")] 
    [SerializeField] public bool isInWorldScene = false;
    

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
    
    void Update()
    {
        HandleInventoryInput();
    }

    private void HandleInventoryInput()
    {
        if (!isInWorldScene) return;
        if (inventoryInput)
        {
            ToggleInventory();
        }

        // Jeśli ekwipunek jest otwarty, zablokuj ruch postaci i myszki
        if (isInventoryOpen)
        {
            UnlockPlayerCursor();
        }
        else
        {
            LockPlayerCursor();
        }
    }
    
    // 
    public void UnlockPlayerCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
       
    }

    // M
    public void LockPlayerCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    
        // playerController.enabled = true;
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryInput = false;
        // inventoryUI.SetActive(isInventoryOpen);  // Aktywuj/dezaktywuj UI ekwipunku
    }
    
}