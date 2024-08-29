using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryUISystem : MonoBehaviour
{
    [SerializeField] MouseItemHolderUI mouseInventoryItem;

    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlotUI, InventorySlot> slotDictionary;
    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlotUI, InventorySlot> SlotDictionary => slotDictionary;


    protected virtual void Start()
    {

    }

    public virtual void InitializeUISystem(InventoryContainer inventoryContainer)
    {
        //f
    }

    public abstract void AssignSlot(InventorySystem invToDisplay);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot) // wartosc slotu /
            {
                slot.Key.UpdateUISlot(updatedSlot); // klucz slotu /reprezentacja w ui reprezentuje wartosc w ui 

            }
        }
    }

    public void SlotClicked(InventorySlotUI clickedSlot)
    {
        Debug.Log("Slot clicked");
  
        // TODO: Jeśli slot ma przedmiot a mysz jest pusta podnieś przedmiot ze slotu/

        if (GlobalInputManager.instance.stackInput &&
            clickedSlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
        {
            mouseInventoryItem.UpdateMouseHolderSlot(halfStackSlot);
            clickedSlot.UpdateUISlot();
            return;
        }
        if (clickedSlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            PickUpItemFromSlot(clickedSlot);
            // : Jeśli Gracz trzyma przycisk np shift / podziel stak
            return;

        }
        // TODO: Jeśli slot jest pusty a mysz ma przedmiot umieść przedmiot w slocie
        if (clickedSlot.AssignedInventorySlot.ItemData == null &&
            mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            PlaceItemInSlot(clickedSlot);
            return;
        }
        {
            
        }
        // TODO: Jeśli slot i mysz mają przedmioty:
     
        //       - Zamień przedmioty jeśli są różne
        
        //wec
        // TODO: Jeśli slot i mysz mają przedmioty
        if (clickedSlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            HandleStackingOrSwapping(clickedSlot);
        }
        
    }
    
    private void PickUpItemFromSlot(InventorySlotUI clickedSlot)
    {
        mouseInventoryItem.UpdateMouseHolderSlot(clickedSlot.AssignedInventorySlot);
        clickedSlot.ClearSlot();
    }

    private void PlaceItemInSlot(InventorySlotUI clickedSlot)
    {
        clickedSlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
        clickedSlot.UpdateUISlot();
        mouseInventoryItem.ClearMouseHolderAssignedSlot();
    }
    
    private void HandleStackingOrSwapping(InventorySlotUI clickedSlot)
    {
        bool isSameItem = clickedSlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
        bool isRoomInStack = clickedSlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack);

        if (isSameItem)
        {
            MergeOrAdjustStack(clickedSlot, isRoomInStack, leftInStack);
        }
        else
        {
            SwapSlots(clickedSlot);
        }
    }

    private void MergeOrAdjustStack(InventorySlotUI clickedSlot, bool isRoomInStack, int leftInStack)
    {
        if (isRoomInStack)
        {
            clickedSlot.AssignedInventorySlot.AddToStack(mouseInventoryItem.AssignedInventorySlot.StackSize);
            clickedSlot.UpdateUISlot();
            mouseInventoryItem.ClearMouseHolderAssignedSlot(); // czyszczenie połączonych stacków
        }
        else
        {
            if (leftInStack < 1)
            {
                SwapSlots(clickedSlot); // stack pełny, zamieniamy przedmioty
            }
            else
            {
                AdjustStacks(clickedSlot, leftInStack);
            }
        }
    }

    private void AdjustStacks(InventorySlotUI clickedSlot, int leftInStack)
    {
        int remainingAmountOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;

        // przykładowo jeśli mamy 5 na myszce a leftInStack na slocie jest 2, wartość pozostanie 3, bo zabieramy 2 z myszki, aby uzupełnić stack
        clickedSlot.AssignedInventorySlot.AddToStack(leftInStack);
        clickedSlot.UpdateUISlot();

        // zaktualizuj mysz z pozostałą ilością przedmiotu
        var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingAmountOnMouse);
        mouseInventoryItem.ClearMouseHolderAssignedSlot();
        mouseInventoryItem.UpdateMouseHolderSlot(newItem);
    }

    
    //sklonuj i zamien
    private void SwapSlots(InventorySlotUI clickedSlot)
    {
        var clonedSlot = mouseInventoryItem.AssignedInventorySlot.Clone();
        mouseInventoryItem.ClearMouseHolderAssignedSlot();

        mouseInventoryItem.UpdateMouseHolderSlot(clickedSlot.AssignedInventorySlot);

        clickedSlot.ClearSlot();
        clickedSlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedSlot.UpdateUISlot();
    }
}