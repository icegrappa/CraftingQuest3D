using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable] // inventory z wlasna metoda add ktora upuszcza item gdy mamy za malo miesjca eq
public class PlayerInventory : InventoryContainer
{
    public static UnityAction OnPlayerInventoryChanged;
    
    public bool AddToInventory(ItemData data, int amount, bool spawnItemOnFail = false)
    {
        if (inventorySystem.AddItem(data, amount))
        {
            return true;
        }

        var transform1 = transform;
        if (spawnItemOnFail)
            //spawn itemu
            Instantiate(data.itemPrefab, transform1.position + transform1.forward * 2f, Quaternion.identity);

        return false;
    }
}