using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class CraftingSuccessEvent : UnityEvent<CraftingRecipe> { }

[System.Serializable]
public class CraftingFailureEvent : UnityEvent<CraftingRecipe> { }

public class CraftingHandler : MonoBehaviour, ICraftingEventHandler
{
    [Header("Identyfikator Eventu")]
    [Tooltip("Unikalny identyfikator dla tego handlera. Jest on używany przez CraftingEventManager do powiązania eventów.")]
    [SerializeField] private string eventIdentifier;

    [Header("Eventy Wywoływane Przy Sukcesie Craftingu")]
    [Tooltip("Event wywoływany przy sukcesie craftingu. Zostanie wywołany, gdy CraftingEventManager powiadomi o sukcesie.")]
    [SerializeField] private CraftingSuccessEvent onCraftingSuccess = new CraftingSuccessEvent();

    [Header("Eventy Wywoływane Przy Niepowodzeniu Craftingu")]
    [Tooltip("Event wywoływany przy niepowodzeniu craftingu. Zostanie wywołany, gdy CraftingEventManager powiadomi o niepowodzeniu.")]
    [SerializeField] private CraftingFailureEvent onCraftingFailure = new CraftingFailureEvent();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(WaitForCraftingEventManager());
    }

    private IEnumerator WaitForCraftingEventManager()
    {
        // Czekamy, aż aktywna scena będzie miała indeks 0/scena swiata i będzie w pełni załadowana
        while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 ||
               !UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }

        // Upewniamy się, że scena jest w pełni załadowana, czekając jeszcze jedną klatkę
        yield return new WaitForEndOfFrame();

        // Czekamy, aż instancja CraftingEventManager będzie dostępna
        while (CraftingEventManager.Instance == null)
        {
            yield return null;
        }

        // Rejestrujemy handler w globalnym CraftingEventManager
        CraftingEventManager.Instance.RegisterEventHandler(eventIdentifier, this);
        Debug.Log("CraftingHandler registered successfully.");
    }



    private void OnDestroy()
    {
        if (CraftingEventManager.Instance != null)
        {
            CraftingEventManager.Instance.UnregisterEventHandler(eventIdentifier);
        }
    }

    public void OnCraftingSuccess(CraftingRecipe recipe)
    {
        Debug.Log($"Crafting Success: {recipe.CraftedItem.name}");
        onCraftingSuccess.Invoke(recipe);
    }

    public void OnCraftingFailure(CraftingRecipe recipe)
    {
        Debug.Log($"Crafting Failed: {recipe.CraftedItem.name}");
        onCraftingFailure.Invoke(recipe);
    }
}