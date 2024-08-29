using System.Collections.Generic;
using UnityEngine;

public class FixedSlotInventoryUI : InventoryUISystem
{
    [SerializeField] private InventorySlotUI[] slots;

    protected override void Start()
    {
        InventoryManager.instance.RegisterFixedSlotInventoryUI(this);
    }
    
    public void SetupInventoryUI(InventorySlotUI[] slotsArray)
    {
        slots = slotsArray; 
    }
    public override void InitializeUISystem(InventoryContainer inventoryContainer)
    {
        if (inventoryContainer != null)
        {
            inventorySystem = inventoryContainer.InventorySystem;

            if (inventorySystem != null)
            {
                inventorySystem.OnInventorySlotChanged += UpdateSlot;
                AssignSlot(inventorySystem);
            }
            else
            {
                Debug.LogWarning($"brak systemu inventory {inventoryContainer} dla {this.gameObject}");
            }
        }
    }

    public override void AssignSlot(InventorySystem invToDisplay)
    {
        slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>();

        if (slots.Length != inventorySystem.InventorySize)
        {
            Debug.LogWarning($"wielkosc slotu poza wielkoscia inventory {this.gameObject}");
        }

        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Initialize(inventorySystem.InventorySlots[i]);
        }
    }
}