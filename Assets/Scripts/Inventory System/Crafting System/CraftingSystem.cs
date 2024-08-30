using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipe> _knownRecipes = new List<CraftingRecipe>(); // Lista znanych przepisów na crafting

    private readonly string[] failureQuotes = 
    {
        "Crafting failed, luck isn't your best friend today.", // tekst przy nieudanym craftingu
        "The stars weren't aligned, crafting failed.",
        "Crafting failed, better luck next time!",
        "Your skill wasn't enough this time, crafting failed.",
        "Crafting failed, perhaps the gods are not on your side."
    };

    private PlayerInventory _playerInventory; // Referencja do ekwipunku gracza

    // Publiczny getter do znanych przepisów
    public List<CraftingRecipe> KnownRecipes => _knownRecipes;

    // Metoda próbująca stworzyć przedmiot na podstawie przepisu
    public (bool isSuccess, string failureReason) TryToCraftAnItem(CraftingRecipe activeRecipe)
    {
        Debug.Log("Interacting with workbench"); // Informacja o rozpoczęciu craftingu

        if (_playerInventory == null)
        {
            Debug.Log("Player inventory is null"); // Błąd brak referencji do ekwipunku gracza
            return (false, "Player inventory is null");
        }

        if (!CheckIfCanCraft(activeRecipe))
        {
            Debug.Log("Not enough resources to craft"); // Błąd brak wystarczających zasobów do craftingu
            activeRecipe.TriggerFailure(); // Wywołanie niepowodzenia craftingu
            return (false, "Not enough resources to craft");
        }

        // obliczam szanse na suckces
        bool isSuccess = Random.value <= activeRecipe.SuccessChance / 100f;
        if (isSuccess)
        {
            // usuwam potrzebne skladniki składników z ekwipunku
            foreach (var ingredient in activeRecipe.Ingredients)
            {
                _playerInventory.InventorySystem.RemoveItemsFromInventory(ingredient.ItemRequired, ingredient.AmountRequired);
            }

            // tutaj dodaje stworzony przedmiot do ekwipunku gracza
            _playerInventory.AddToInventory(activeRecipe.CraftedItem, activeRecipe.CraftedAmount, spawnItemOnFail: true);
            activeRecipe.TriggerSuccess(); // Wywołanie sukcesu craftingu
            Debug.Log("Item crafted successfully");
            return (true, string.Empty); // Sukces, brak powodu porażki
        }
        else
        {
            activeRecipe.TriggerFailure(); // Wywołanie niepowodzenia craftingu
            
            // Usunięcie składników nawet przy nieudanym craftingu
            foreach (var ingredient in activeRecipe.Ingredients)
            {
                _playerInventory.InventorySystem.RemoveItemsFromInventory(ingredient.ItemRequired, ingredient.AmountRequired);
            }

            // Wybór losowego cytatu na temat porażki
            string randomQuote = failureQuotes[Random.Range(0, failureQuotes.Length)];
            return (false, randomQuote);
        }
    }

    // Sprawdzenie, czy można wykonać crafting na podstawie dostępnych zasobów
    private bool CheckIfCanCraft(CraftingRecipe activeRecipe)
    {
        var itemsHeld = _playerInventory.InventorySystem.GetAllItemsHeld();

        foreach (var ingredient in activeRecipe.Ingredients)
        {
            if (!itemsHeld.TryGetValue(ingredient.ItemRequired, out int amountHeld) || amountHeld < ingredient.AmountRequired)
            {
                return false; // Brak wystarczających zasobów
            }
        }

        return true; // Wystarczające zasoby do wykonania craftingu
    }

    // ustawia inventory
    public void SetPlayerInventory(PlayerInventory inventory)
    {
        _playerInventory = inventory;
    }
}
