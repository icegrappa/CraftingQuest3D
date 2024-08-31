using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager instance;

    [SerializeField] private GameObject inventorySlotPrefab; // Prefab do generowania UI slotów inv
    [SerializeField] private GameObject helpWindowParent; 
    [SerializeField] private GameObject escapeWindowParent;
    [SerializeField] private CraftingSystem _currentCraftingSystem;
    [SerializeField] private CraftingUISystem _craftingWindowParent;
    [SerializeField] private InteractionUISystem _interactionUISystem;
   

    private InventoryContainer inventoryContainer;
    [SerializeField] private FixedSlotInventoryUI _fixedSlotInventoryUI;

    [SerializeField]  private bool _wasInventoryOpen;
    private bool wasInputActive = false;
    private bool previousCraftingState = false;
    private bool previousHelpState = false;


    private void OnEnable()
    {
        //CraftingSystem.OnCraftingDisplayRequested += DisplayCraftingWindow;
    }

    private void OnDisable()
    {
        //CraftingSystem.OnCraftingDisplayRequested -= DisplayCraftingWindow;
    }


    private void Awake()
    {
        // Zapewniamy, że instancja jest singletonem
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _craftingWindowParent.gameObject.SetActive(false);
   
        HideInteractionPrompt();
        DisableHelpWindow();
        DisableEscapeWindow();
    }

    private void Start()
    {
        InitializeFixedSlotInventoryUI();
    }
    

    private void Update()
    {
        CheckInventoryOpenStatus();
        CheckPendingInteractionStatus();
        HandleHelpOpenStatus();
        HandleVisibilityOfCrafting();
        HandleVisibilityOfEscapeWindow();
    }

    private void CheckInventoryOpenStatus()
    {
        // Check if the inventory state has changed
        if (GlobalInputManager.instance.isInventoryOpen != _wasInventoryOpen)
        {
            _wasInventoryOpen = GlobalInputManager.instance.isInventoryOpen;

            if (_wasInventoryOpen)
            {
                DisplayCraftingWindow();
            }
            else
            {
                _craftingWindowParent.CleanUISystem();
                _craftingWindowParent.gameObject.SetActive(false);
            }
        }
    }

    private void CheckPendingInteractionStatus()
    {
        // Resetujemy stan interakcji jeśli nie ma żadnych interaktywnych obiektów w pobliżu
        if (!InteractableManager.HasInteractables())
        {
            GlobalInputManager.instance.interactInput = false;
            HideInteractionPrompt();
        }
    }

    public void InitializeAllInventories(InventoryContainer inventoryContainer)
    {
        // Inicjalizujemy UI systemu inv jeśli jest przypisany
        if (_fixedSlotInventoryUI != null)
        {
            _fixedSlotInventoryUI.InitializeUISystem(inventoryContainer);
            _currentCraftingSystem.SetPlayerInventory(inventoryContainer as PlayerInventory);
        }
    }


    // Rejestruje instancję fixedslot .

    public void InitializeFixedSlotInventoryUI()
    {
        CreateAndAssignSlots(10); // Tworzymy i przypisujemy sloty inwentarza
        DisablePlayerInventoryUI();
    }


    // Tworzy i przypisuje instancje InventorySlotUI jako dzieci obiektu FixedSlotInventoryUI.
    public void CreateAndAssignSlots(int slotCount)
    {
        var inventorySlots = new InventorySlotUI[slotCount];

        for (var i = 0; i < slotCount; i++)
        {
            // Instancjujemy prefab jako dziecko w canvas
            var slotInstance = Instantiate(inventorySlotPrefab, _fixedSlotInventoryUI.transform);

            // Pobieramy komponent InventorySlotUI i przypisujemy go do array
            inventorySlots[i] = slotInstance.GetComponent<InventorySlotUI>();
        }

        // Przekazujemy array do FixedSlotInventoryUI w celu ustawienia
        _fixedSlotInventoryUI.SetupInventoryUI(inventorySlots);
    }

    //wyswietla okno crafitnu
    public void DisplayCraftingWindow()
    {
        _craftingWindowParent.gameObject.SetActive(true);
        _craftingWindowParent.DisplayCraftingWindow(_currentCraftingSystem);
    }
    public void CloseCraftingWindow()
    {
        _craftingWindowParent.gameObject.SetActive(false);
        _craftingWindowParent.DisplayCraftingWindow(_currentCraftingSystem);
    }
    
    //pokazuje prompt interakcji np przy podnoszeniu itemu

    public void ShowInteractionPrompt(ItemData itemData, int amount, string interactionMessage)
    {
        if (_interactionUISystem.IsItemPopupActive) return;
        _interactionUISystem.ShowItemPopup(itemData, amount, interactionMessage);
    }

    //chowa go
    public void HideInteractionPrompt()
    {
        _interactionUISystem.ClearItemPopup();
    }

    // sloty sa widoczne
    public void EnablePlayerInventoryUI()
    {
        _fixedSlotInventoryUI.MakeSlotsVisible();
    }

    public void DisablePlayerInventoryUI()
    {
        _fixedSlotInventoryUI.MakeSlotsInvisible();
    }
    
    public void DisableHelpWindow()
    {
        helpWindowParent.SetActive(false);
    }
    
    public void DisableEscapeWindow()
    {
        escapeWindowParent.SetActive(false);
    }

    public void ToggleHelpWindow()
    {
        // Toggle the active state of the help window
        helpWindowParent.SetActive(!helpWindowParent.activeSelf);
    }

    private void HandleHelpOpenStatus()
    {
        if (GlobalInputManager.instance.helpInput)
        {
            ToggleHelpWindow();
            GlobalInputManager.instance.helpInput = false;
        }
    }

    public void LockPlayerInputUI()
    {
        GlobalInputManager.instance.LockPlayerCursor();
    }

    
    public void HandleVisibilityOfCrafting()
    {
        bool isInputActive = GlobalInputManager.instance.deleteItemUIInput;

        if (isInputActive && !wasInputActive)
        {
            // Input just became active, save current states
            previousCraftingState = _craftingWindowParent.gameObject.activeSelf;
            previousHelpState = helpWindowParent.activeSelf;

            // Hide both elements
            _craftingWindowParent.gameObject.SetActive(false);
            helpWindowParent.SetActive(false);
        }
        else if (!isInputActive && wasInputActive)
        {
            // Input just became inactive, restore previous states
            _craftingWindowParent.gameObject.SetActive(previousCraftingState);
            helpWindowParent.SetActive(previousHelpState);
        }

        // Update the previous input state for next frame
        wasInputActive = isInputActive;
    }

    private void HandleVisibilityOfEscapeWindow()
    {
        if (GlobalInputManager.instance.escapeInput)
        {
            escapeWindowParent.gameObject.SetActive(!escapeWindowParent.gameObject.activeSelf);
            GlobalInputManager.instance.UnlockPlayerCursor();
            GlobalInputManager.instance.escapeInput = false;
            GlobalInputManager.instance.hasEscapeScreen = true;
        }
    }

    public virtual void LoadMainMenuFromGameManager()
    {
        GlobalInputManager.instance.hasEscapeScreen = false;
        GameManager.instance.LoadMainMenuScene();
        GlobalInputManager.instance.isInWorldScene = false;
        GlobalInputManager.instance.UnlockPlayerCursor();
    }

    
}