using System.Collections.Generic;
using UnityEngine;

public class CraftingEventManager : MonoBehaviour
{
    private static CraftingEventManager _instance;

    [Header("DEBUG ONLY")]
    [Tooltip("Ten serializowany dictionary służy do celów debugowania. Kluczem jest unikalny identyfikator eventu, a wartością jest obiekt implementujący ICraftingEventHandler. " +
             "System działa w ten sposób, że gdy CraftingRecipe wywoła event (sukces lub niepowodzenie), wysyła identyfikator do CraftingEventManager, " +
             "który następnie wyszukuje odpowiedniego handlera w tym słowniku i wywołuje jego metodę OnCraftingSuccess lub OnCraftingFailure.")]
    [SerializeField] private SerializableDictionary<string, ICraftingEventHandler> _eventHandlersDebug = new SerializableDictionary<string, ICraftingEventHandler>();

    // przechowuje id do obiektów implementujących ICraftingEventHandler
    private Dictionary<string, ICraftingEventHandler> _eventHandlers;

    public static CraftingEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CraftingEventManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CraftingEventManager");
                    _instance = go.AddComponent<CraftingEventManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _eventHandlers = new Dictionary<string, ICraftingEventHandler>();
        
        DontDestroyOnLoad(gameObject);  
    }

    // Rejestruje handlera eventu za pomocą unikalnego id
    public void RegisterEventHandler(string identifier, ICraftingEventHandler handler)
    {
        if (_eventHandlers == null)
        {
            _eventHandlers = new Dictionary<string, ICraftingEventHandler>();
        }

        if (_eventHandlers.TryAdd(identifier, handler))
        {
            _eventHandlersDebug.Add(identifier, handler); // Dodajemy do słownika debugowania
            Debug.Log($"Handler for {identifier} added successfully.");
        }
        else
        {
            Debug.LogWarning($"Handler for {identifier} already exists.");
        }
    }


    // Wyrejestrowanie handlera eventu
    public void UnregisterEventHandler(string identifier)
    {
        if (_eventHandlers.ContainsKey(identifier))
        {
            _eventHandlers.Remove(identifier);
            _eventHandlersDebug.Remove(identifier);  // Usuwamy ze słownika debugowania
        }
    }

   
    // Wywołanie eventu sukcesu craftingu
    public void TriggerCraftingSuccess(string identifier, CraftingRecipe recipe)
    {
        if (_eventHandlers.TryGetValue(identifier, out ICraftingEventHandler handler))
        {
            Debug.Log($"Triggering crafting success for identifier: {identifier} with recipe: {recipe.CraftedItem.ItemName}");
            handler.OnCraftingSuccess(recipe);
        }
        else
        {
            Debug.LogWarning($"No handler found for crafting success with identifier: {identifier}");
        }
    }

// Wywołanie eventu niepowodzenia craftingu
    public void TriggerCraftingFailure(string identifier, CraftingRecipe recipe)
    {
        if (_eventHandlers.TryGetValue(identifier, out ICraftingEventHandler handler))
        {
            Debug.Log($"Triggering crafting failure for identifier: {identifier} with recipe: {recipe.CraftedItem.ItemName}");
            handler.OnCraftingFailure(recipe);
        }
        else
        {
            Debug.LogWarning($"No handler found for crafting failure with identifier: {identifier}");
        }
    }

}
