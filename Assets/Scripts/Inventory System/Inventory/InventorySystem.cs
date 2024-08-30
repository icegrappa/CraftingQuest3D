using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventorySystem
{
    // prywatna lista slotów inventoryy
    [SerializeField] private List<InventorySlot> inventorySlots;

    // publiczny getter dla inventorySlots, umożliwiający tylko odczyt
    public IReadOnlyList<InventorySlot> InventorySlots => inventorySlots;

    // publiczna właściwość zwracająca liczbę slotów inwentarza
    public int InventorySize => inventorySlots.Count;

    // wydarzenie, które uruchamia się, gdy slot w inventory ulegnie zmianie
    public UnityAction<InventorySlot> OnInventorySlotChanged;

    // incixzjalizacja inventory z określoną liczbą slotów
    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);
        for (var i = 0; i < size; i++) inventorySlots.Add(new InventorySlot());
    }

    public bool AddItem(ItemData item, int amount)
    {
        // Sprawdzamy czy mamy item w inventory?
        if (ContainsItem(item, out var slots))
            // Szukamy miejsca w istniejących slotach
            foreach (var slot in slots)
                if (slot.RoomLeftInStack(amount))
                {
                    slot.AddToStack(amount); // Dodajemy itemy do stacka
                    OnInventorySlotChanged?.Invoke(slot); // Informujemy ze slot sie zmienil
                    return true; // Konczymy bo dodano item nie chcemy sprawdzac dalej
                }

        // Sprawdzamy czy mamy wolny slot na nowy item
        if (HasFreeSlot(out var freeSlot))
        {
            freeSlot.UpdateInventorySlot(item, amount); // Aktualizujemy wolny slot
            OnInventorySlotChanged?.Invoke(freeSlot); // Informujemy ze slot sie zmienil
            return true; // |\
        }

        return false; // Nie udalo sie dodac itemu brak miejsca
    }

    public bool ContainsItem(ItemData item, out List<InventorySlot> slots)
    {
        slots = new List<InventorySlot>();

        // Przeszukujemy sloty w poszukiwaniu itemu
        foreach (var slot in inventorySlots)
            if (slot.ItemData == item)
                slots.Add(slot); // Dodajemy slot do listy znalezionych slotow

        return slots.Count > 0; // Zwracamy czy znaleziono jakiekolwiek sloty
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = null;

        // Przeszukujemy sloty w poszukiwaniu wolnego miejsca
        foreach (var slot in inventorySlots)
            if (slot.ItemData == null)
            {
                freeSlot = slot; // Zwracamy wolny slot
                return true;
            }

        return false;
    }

   /// <summary>
///     Zwraca dictionary zawiera unikalne przedmioty w ekwipunku oraz ich ilość, ignorując rozmiar stack.
/// </summary>
/// <returns>DistinctItems - słownik unikalnych przedmiotów i ich ilości</returns>
public Dictionary<ItemData, int> GetAllItemsHeld()
{
    var distinctItems = new Dictionary<ItemData, int>();

    // Przechodzimy przez wszystkie sloty ekwipunku
    foreach (var item in inventorySlots)
    {
        if (item.ItemData == null) continue; // Pomijamy puste sloty

        // Sprawdzamy czy dany przedmiot już jest w dictionary
        if (!distinctItems.ContainsKey(item.ItemData))
            distinctItems.Add(item.ItemData, item.StackSize); // nie, dodajemy 
        else
            distinctItems[item.ItemData] += item.StackSize; //zwiększamy liczbę o rozmiar stacka
    }

    return distinctItems;
}

public void RemoveItemsFromInventory(ItemData data, int amount)
{
    Debug.Log($"Próba usunięcia {amount} sztuk {data.ItemName} z ekwipunku.");

    // Sprawdzamy czy przedmiot jest w ekwipunku/
    if (ContainsItem(data, out var invSlot))
    {
        // szuakmy we wszystkich slotac z danym przedmiotem
        foreach (var slot in invSlot)
        {
            var stackSize = slot.StackSize;
            Debug.Log($"Znaleziono slot z {stackSize} sztukami {data.ItemName}.");

            // Określamy, ile można usunąć z tego slotu
            int amountToRemove = Mathf.Min(amount, stackSize);
            slot.RemoveFromStack(amountToRemove); // Usuwamy przedmiot ze slotu
            amount -= amountToRemove; // Zmniejszamy pozostałą ilość do usunięcia

            Debug.Log($"Usunięto {amountToRemove} sztuk {data.ItemName} ze slotu. Pozostało do usunięcia: {amount}");

            OnInventorySlotChanged?.Invoke(slot); // Aktualizujemy stan slotu

            // Jeśli usunięto wymaganą ilość, kończymy
            if (amount <= 0)
            {
                Debug.Log($"Pomyślnie usunięto wymaganą ilość {data.ItemName} z ekwipunku.");
                break;
            }
        }
    }
    else
    {
        Debug.LogWarning($"Przedmiot {data.ItemName} nie znaleziono w ekwipunku."); // Przedmiot nie został znaleziony
    }

    // Sprawdzamy, czy pozostało jeszcze coś do usunięcia
    if (amount > 0)
    {
        Debug.LogWarning($"Brakuje {data.ItemName} w ekwipunku, aby usunąć wymaganą ilość. Pozostało: {amount}");
    }
}


}