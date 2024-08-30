using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedSlotInventoryUI : InventoryUISystem
{
    [SerializeField] private InventorySlotUI[] slots; 
    [SerializeField] private Image _inventoryUIBackground; 
    
    private void Awake()
    {
        _inventoryUIBackground = GetComponent<Image>(); 
    }

    // ukrywa wszystkie sloty ekwipunku
    public void MakeSlotsInvisible()
    {
        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(false); // Ukrywa każdy slot
        }
        _inventoryUIBackground.enabled = false; // Wyłącza tło UI
    }

  
    public void MakeSlotsVisible()
    {
        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(true); 
        }
        _inventoryUIBackground.enabled = true; 
    }

    // Metoda do ustawienia slotów w ekwipunku
    public void SetupInventoryUI(InventorySlotUI[] slotsArray)
    {
        slots = slotsArray; // Przypisuje stworzone instancje
    }

    // Inicjalizuje system UI dla ekwipunku
    public override void InitializeUISystem(InventoryContainer inventoryContainer)
    {
        if (inventoryContainer != null)
        {
            inventorySystem = inventoryContainer.InventorySystem;

            if (inventorySystem != null)
            {
                inventorySystem.OnInventorySlotChanged += UpdateSlot; // Dodaje listener do zdarzenia zmiany slotu
                AssignSlot(inventorySystem); // Przypisuje sloty na podstawie systemu inventory
            }
            else
            {
                Debug.LogWarning($"Brak systemu inventory w {inventoryContainer} dla {this.gameObject}"); 
            }
        }
    }

    // Przypisuje sloty ekwipunku na podstawie przekazanego systemu inventory
    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>(); // Inicjalizacja dictionar slotów

        if (slots.Length != inventorySystem.InventorySize)
        {
            Debug.LogWarning($"Wielkość slotów wykracza poza wielkość inventory w {this.gameObject}"); // czy liczba slowotw pasuje ro zormiaru inventory ?
        }

        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]); // Dodaje sloty do dictionary
            slots[i].Initialize(inventorySystem.InventorySlots[i]); // inicijualizuje kazdy slot dla odpowiedniego InventorySlot
        }
    }
}
