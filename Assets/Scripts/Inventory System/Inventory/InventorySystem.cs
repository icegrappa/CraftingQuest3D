using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        // Sprawdzamy czy mamy item w inventory
        if (ContainsItem(item, out List<InventorySlot> slots))
        {
            // Szukamy miejsca w istniejących slotach
            foreach (var slot in slots)
            {
                if (slot.RoomLeftInStack(amount))
                {
                    slot.AddToStack(amount); // Dodajemy itemy do stacka
                    OnInventorySlotChanged?.Invoke(slot); // Informujemy ze slot sie zmienil
                    return true; // Konczymy bo dodano item
                }
            }
        }

        // Sprawdzamy czy mamy wolny slot na nowy item
        if (HasFreeSlot(out InventorySlot freeSlot))
        {
            freeSlot.UpdateInventorySlot(item, amount); // Aktualizujemy wolny slot
            OnInventorySlotChanged?.Invoke(freeSlot); // Informujemy ze slot sie zmienil
            return true; // Konczymy bo dodano item
        }

        return false; // Nie udalo sie dodac itemu brak miejsca
    }

    public bool ContainsItem(ItemData item, out List<InventorySlot> slots)
    {
        slots = new List<InventorySlot>();

        // Przeszukujemy sloty w poszukiwaniu itemu
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemData == item)
            {
                slots.Add(slot); // Dodajemy slot do listy znalezionych slotow
            }
        }

        return slots.Count > 0; // Zwracamy czy znaleziono jakiekolwiek sloty
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = null;

        // Przeszukujemy sloty w poszukiwaniu wolnego miejsca
        foreach (var slot in inventorySlots)
        {
            if (slot.ItemData == null)
            {
                freeSlot = slot; // Zwracamy wolny slot
                return true;
            }
        }

        return false; 
    }


}