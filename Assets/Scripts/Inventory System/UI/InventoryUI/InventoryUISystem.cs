using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryUISystem : MonoBehaviour
{
    [SerializeField] private MouseItemHolderUI mouseInventoryItem;

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
            if (slot.Value == updatedSlot) // wartosc slotu /
                slot.Key.UpdateUISlot(updatedSlot); // klucz slotu /reprezentacja w ui reprezentuje wartosc w ui 
    }

    public void SlotClicked(InventorySlotUI clickedSlot)
    {
        Debug.Log("Slot clicked");

        // TODO: Jeśli slot ma przedmiot a mysz jest pusta podnieś przedmiot ze slotu/

        if (GlobalInputManager.instance.stackInput &&
            clickedSlot.AssignedInventorySlot.SplitStack(out var halfStackSlot))
        {
            mouseInventoryItem.UpdateMouseHolderSlot(halfStackSlot);
            clickedSlot.UpdateUISlot();
            return;
        }

        if (clickedSlot.AssignedInventorySlot.ItemData != null &&
            mouseInventoryItem.AssignedInventorySlot.ItemData == null)
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
        if (clickedSlot.AssignedInventorySlot.ItemData != null &&
            mouseInventoryItem.AssignedInventorySlot.ItemData != null) HandleStackingOrSwapping(clickedSlot);
    }

    private void PickUpItemFromSlot(InventorySlotUI clickedSlot)
    {
        mouseInventoryItem.UpdateMouseHolderSlot(clickedSlot
            .AssignedInventorySlot); // Przenosi przedmiot z klikniętego slotu do kursora (myszy)
        clickedSlot.ClearSlot(); // Czyści slot po zabraniu przedmiotu
    }

    private void PlaceItemInSlot(InventorySlotUI clickedSlot)
    {
        clickedSlot.AssignedInventorySlot.AssignItem(mouseInventoryItem
            .AssignedInventorySlot); // Przenosi przedmiot z kursora do klikniętego slotu
        clickedSlot.UpdateUISlot(); // Aktualizuje UI slotu, by odzwierciedlić nowy przedmiot
        mouseInventoryItem.ClearMouseHolderAssignedSlot(); // Czyści kursor po umieszczeniu przedmiotu
    }

    private void HandleStackingOrSwapping(InventorySlotUI clickedSlot)
    {
        var isSameItem = clickedSlot.AssignedInventorySlot.ItemData ==
                         mouseInventoryItem.AssignedInventorySlot
                             .ItemData; // Sprawdza czy przedmiot w slocie i na kursore to ten sam typ
        var isRoomInStack =
            clickedSlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize,
                out var leftInStack); // Sprawdza czy jest miejsce na dodanie więcej przedmiotów do stacka

        if (isSameItem)
            MergeOrAdjustStack(clickedSlot, isRoomInStack,
                leftInStack); // Łączy lub dostosowuje stacki jeśli to ten sam przedmiot
        else
            SwapSlots(clickedSlot); // Zamienia przedmioty w slotach jeśli to różne przedmioty
    }

    private void MergeOrAdjustStack(InventorySlotUI clickedSlot, bool isRoomInStack, int leftInStack)
    {
        if (isRoomInStack)
        {
            clickedSlot.AssignedInventorySlot.AddToStack(mouseInventoryItem.AssignedInventorySlot
                .StackSize); // Dodaje przedmiot z kursora do stacka w slocie
            clickedSlot.UpdateUISlot(); // Aktualizuje UI slotu
            mouseInventoryItem.ClearMouseHolderAssignedSlot(); // Czyści kursor po połączeniu stacków
        }
        else
        {
            if (leftInStack < 1)
                SwapSlots(clickedSlot); // Jeśli stack pełny, zamienia przedmioty
            else
                AdjustStacks(clickedSlot, leftInStack); // Dostosowuje stacki jeśli jest trochę miejsca
        }
    }


    private void AdjustStacks(InventorySlotUI clickedSlot, int leftInStack)
    {
        var remainingAmountOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;

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