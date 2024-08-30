using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CraftingSuccessEvent : UnityEvent<CraftingRecipe> { }

[System.Serializable]
public class CraftingFailureEvent : UnityEvent<CraftingRecipe> { }

public class CraftingHandler : MonoBehaviour, ICraftingEventHandler
{
    [Header("Identyfikator Eventu")]
    [Tooltip("Unikalny identyfikator dla tego handlera. Jest on używany przez CraftingEventManager do powiązania eventów.")]
    [SerializeField] private string eventIdentifier; // Unikalny identyfikator dla tego handlera

    [Header("Eventy Wywoływane Przy Sukcesie Craftingu")]
    [Tooltip("Event wywoływany przy sukcesie craftingu. Zostanie wywołany, gdy CraftingEventManager powiadomi o sukcesie.")]
    [SerializeField] private CraftingSuccessEvent onCraftingSuccess = new CraftingSuccessEvent();

    [Header("Eventy Wywoływane Przy Niepowodzeniu Craftingu")]
    [Tooltip("Event wywoływany przy niepowodzeniu craftingu. Zostanie wywołany, gdy CraftingEventManager powiadomi o niepowodzeniu.")]
    [SerializeField] private CraftingFailureEvent onCraftingFailure = new CraftingFailureEvent();
    private void Start()
    {
        // Rejestrujemy handler w globalnym CraftingEventManageędzie nas powiadamiał o odpowiednich eventach
        CraftingEventManager.Instance.RegisterEventHandler(eventIdentifier, this);
        DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        CraftingEventManager.Instance.UnregisterEventHandler(eventIdentifier);
    }

    public void OnCraftingSuccess(CraftingRecipe recipe)
    {
        Debug.Log($"Crafting Success: {recipe.CraftedItem.name}");
        // Wywołanie eventu sukce który może być powiązany z różnymi akcjami w edytorze
        onCraftingSuccess.Invoke(recipe);
    }

    public void OnCraftingFailure(CraftingRecipe recipe)
    {
        Debug.Log($"Crafting Failed: {recipe.CraftedItem.name}");
        onCraftingFailure.Invoke(recipe);
    }
}