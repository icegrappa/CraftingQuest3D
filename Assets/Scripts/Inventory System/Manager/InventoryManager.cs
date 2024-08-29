using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField] private GameObject inventorySlotPrefab; // Prefab do generowania UI slotów inv

    private InventoryContainer inventoryContainer;
    private FixedSlotInventoryUI _fixedSlotInventoryUI;

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
    }

    public void InitializeAllInventories(InventoryContainer inventoryContainer)
    {
        // Inicjalizujemy UI systemu inv jeśli jest przypisany
        if (_fixedSlotInventoryUI != null)
        {
            _fixedSlotInventoryUI.InitializeUISystem(inventoryContainer);
        }
    }
  

    // Rejestruje instancję fixedslot .
   
    public void RegisterFixedSlotInventoryUI(FixedSlotInventoryUI uiInstance)
    {
        _fixedSlotInventoryUI = uiInstance;
        CreateAndAssignSlots(10); // Tworzymy i przypisujemy sloty inwentarza
    }

   
    // Tworzy i przypisuje instancje InventorySlotUI jako dzieci obiektu FixedSlotInventoryUI.
    public void CreateAndAssignSlots(int slotCount)
    {
        InventorySlotUI[] inventorySlots = new InventorySlotUI[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            // Instancjujemy prefab jako dziecko w canvas
            GameObject slotInstance = Instantiate(inventorySlotPrefab, _fixedSlotInventoryUI.transform);

            // Pobieramy komponent InventorySlotUI i przypisujemy go do array
            inventorySlots[i] = slotInstance.GetComponent<InventorySlotUI>();
        }

        // Przekazujemy array do FixedSlotInventoryUI w celu ustawienia
        _fixedSlotInventoryUI.SetupInventoryUI(inventorySlots);
    }
}
