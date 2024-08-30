using System;
using TMPro;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int stackSize; // liczba przedmiotów w slocie

    // Właściwości tylko do odczytu
    public ItemData ItemData => itemData;
    public int StackSize => stackSize;

    // Konstruktor przyjmujący dane przedmiotu i jego ilość
    public InventorySlot(ItemData source, int amount)
    {
        itemData = source;
        stackSize = amount;
    }
    
    // zwraca kop[oe tego obiektu z aktualnym data i sizem stacka
    public InventorySlot Clone()
    {
        return new InventorySlot(this.ItemData, this.StackSize);
    }

    // Domyślny konstruktor, inicjalizujący pusty slot
    public InventorySlot()
    {
        itemData = null;
        stackSize = -1;
    }

    // Metoda czyszcząca slot, ustawiając go na null i stackSize na -1
    public void ClearSlot()
    {
        itemData = null;
        stackSize = -1;
    }

    public void AssignItem(InventorySlot inventorySlot)
    {
        if (itemData == inventorySlot.ItemData)
        {
            AddToStack(inventorySlot.stackSize);
        }
        else
        {
            itemData = inventorySlot.ItemData;
            stackSize = 0;
            AddToStack(inventorySlot.stackSize);
        }
        
        // jezeli to nie ten sam item zamien
    }

    public void UpdateInventorySlot(ItemData data, int amount)
    {
        itemData = data;
        stackSize = amount;
    }

    // Sprawdza czy jest wystarczająco miejsca w slocie na dodanie przedmiotów
    public bool RoomLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = itemData.MaxStackSize - stackSize;
        return RoomLeftInStack(amountToAdd);
    }

    // Sprawdza, czy można dodać określoną ilość przedmiotów do stosu
    public bool RoomLeftInStack(int amountToAdd)
    {
        return stackSize + amountToAdd <= itemData.MaxStackSize;
    }

    // Dodaje określoną ilość przedmiotów do stosu
    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    // Usuwa określoną ilość przedmiotów ze stosu
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
        if (stackSize <= 0)
        {
            ClearSlot(); // Clear the slot when the stack size reaches zero or below
        }
    }

    public bool SplitStack(out InventorySlot splitStack)
    {
        if (stackSize <= 1)
        {
            splitStack = null;
            return false;
        }

        int halfStack = Mathf.RoundToInt((stackSize / 2));
        RemoveFromStack(halfStack);

        splitStack = new InventorySlot(itemData, halfStack);
        
        // jezeli mamy 4 itemy i trzymamy stack to podzielimy ten stack przez 2 
        return true;
    }
}