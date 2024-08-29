using System.Collections.Generic;
using UnityEngine;

//statyczna klasa do zarzadzania aktywnymi interakcjami
public static class InteractableManager
{
    private static HashSet<InteractableItem> activeInteractables = new HashSet<InteractableItem>();

    public static bool AddInteractable(InteractableItem item)
    {
        return activeInteractables.Add(item);
    }

    public static bool RemoveInteractable(InteractableItem item)
    {
        return activeInteractables.Remove(item);
    }

    public static bool HasInteractables()
    {
        return activeInteractables.Count > 0;
    }
}