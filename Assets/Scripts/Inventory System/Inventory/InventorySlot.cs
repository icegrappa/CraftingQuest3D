using System;
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
    }
}