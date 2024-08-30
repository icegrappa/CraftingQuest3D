using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// Klasa zarzzadza UI craftingu
// Odpowiada za wyświetlanie przepisów, składników oraz wyników craftingu  .

public class CraftingUISystem : MonoBehaviour
{
    [Header("Recipe List Window")]
    [SerializeField] private GameObject _recipeListPanel;
    [SerializeField] private CraftListItemUI _craftListItemUIPrefab;

    [Header("Ingredient Window")]
    [SerializeField] private IngredientSlotUI _ingredientSlotPrefab;
    [SerializeField] private Transform _ingredientGrid;
    [SerializeField] private Button _increaseCraftAmountButton;
    [SerializeField] private Button _decreaseCraftAmountButton;
    [SerializeField] private TextMeshProUGUI _desiredCraftAmountText;

    [Header("Item Display Section")]
    [SerializeField] private Image _itemPreviewSprite;
    [SerializeField] private TextMeshProUGUI _chanceValueText; // Szansa na udane wykonanie przedmiotu
    [SerializeField] private Button _craftButton; // Przycisk do rozpoczęcia craftingu
    [SerializeField] private TextMeshProUGUI _itemPreviewName;
    [SerializeField] private TextMeshProUGUI _itemPreviewDescription;
    
    [SerializeField] private GameObject _itemDescriptionPanel; 
    [SerializeField] private GameObject _recipeText; 
    [SerializeField] private GameObject _chanceHolder; 
    
    [Header("Crafting Output Window")]
    [SerializeField] private GameObject _craftingOutputWindow; // Okno wyników craftingu
    [SerializeField] private TextMeshProUGUI _craftingText; // Tekst informujący o wyniku
    [SerializeField] private Image _itemImage; // Obrazek przedmiotu w oknie wyników
    [SerializeField] private Button _closeButton; // Przycisk zamykający okno wyników

    private int _desiredCraftAmount = 1; // Ilość przedmiotów do stworzenia
    private List<IngredientSlotUI> _ingredientSlots = new List<IngredientSlotUI>();

    private CraftingSystem _craftingSystem;
    private CraftingRecipe _chosenRecipe;

    private void Awake()
    {
        InitializeButtons(); //
        ResetCraftAmount(); // Resetuje ilość przedmiotów do craftingu na 1
        DeactivateConstantUI(); // Dezaktywuje niektóre elementy UI na starcie
        _craftingOutputWindow.SetActive(false); // Ukrywa okno wyników na starcie
    }

    private void ActivateConstantUI()
    {
        // Aktywuje elementy UI, które są zawsze wyświetlane
        _itemDescriptionPanel.SetActive(true);
        _chanceHolder.SetActive(true);
        _recipeText.SetActive(true);
    }
    
    private void DeactivateConstantUI()
    {
        // Dezaktywuje elementy UI, które nie są potrzebne od razu
        _itemDescriptionPanel.SetActive(false);
        _chanceHolder.SetActive(false);
        _recipeText.SetActive(false);
    }
    
    public void CleanUISystem()
    {
        // Czyści UI i resetuje wartości do początkowych\
        ClearRecipeList();
        ResetCraftAmount();
        DeactivateConstantUI();
        InitializeButtons();
    }
    
    private void InitializeButtons()
    {
        // Ustawia działanie przycisków w interfejsie
        _decreaseCraftAmountButton.onClick.RemoveAllListeners();
        _increaseCraftAmountButton.onClick.RemoveAllListeners();

        _decreaseCraftAmountButton.gameObject.SetActive(false);
        _increaseCraftAmountButton.gameObject.SetActive(false);
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(CloseCraftingOutputWindow);
    }

    private void ResetCraftAmount()
    {
        // Resetuje ilość craftowanych przedmiotów na 1
        _desiredCraftAmount = 1;
        _desiredCraftAmountText.text = string.Empty;
    }

    private void ChangeCraftAmount(int amount)
    {
        // Zmienia ilość craftowanych przedmiotów, ale nie pozwala na mniej niż 1 na logike zawsze chcemy tworzyc conajmniej 1 sztuke itemu 
        _desiredCraftAmount = Mathf.Max(1, _desiredCraftAmount + amount);
        _desiredCraftAmountText.text = _desiredCraftAmount.ToString();

        RefreshIngredientDisplay(); // Odświeża listę składników
    }

    public void DisplayCraftingWindow(CraftingSystem craftingSystem)
    {
        // Wyświetla okno craftingu
        _craftingSystem = craftingSystem;
        ClearItemPreview(); // Czyści podgląd przedmiotu
        PopulateRecipeList(); // Wypełnia listę przepisów
    }

    private void ClearRecipeList()
    {
        // Czyści listę przepisów z interfejsu
        if (_ingredientSlots == null) return;

        /*
        foreach (var slot in _ingredientSlots)
        {
            Destroy(slot.gameObject);
        }
        */
        _ingredientSlots.Clear();
        _ingredientSlots = null;
        _ingredientSlots = new List<IngredientSlotUI>();
    }

    private void PopulateRecipeList()
    {
        // Uzupełnia listę przepisów na podstawie dostępnych przepisów w systemie
        ClearSlots(_recipeListPanel.transform);
        foreach (var recipe in _craftingSystem.KnownRecipes)
        {
            var recipeSlot = Instantiate(_craftListItemUIPrefab, _recipeListPanel.transform);
            recipeSlot.Init(recipe, this);
        }
    }

    private void ClearSlots(Transform parentTransform)
    {
        // Usuwa wszystkie elementy UI w danym transformie
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void ClearItemPreview()
    {
        // Czyści podgląd przedmiotu
        _itemPreviewSprite.sprite = null;
        _itemPreviewSprite.color = Color.clear;
        _itemPreviewName.text = string.Empty;
        _itemPreviewDescription.text = string.Empty;
    }

    private void DisplayItemPreview(ItemData itemData)
    {
        // Wyświetla podgląd wybranego przedmiotu
        _itemPreviewSprite.sprite = itemData.Icon;
        _itemPreviewSprite.color = Color.white;
        _itemPreviewName.text = itemData.ItemName;
        _itemPreviewDescription.text = itemData.ItemInfo;
    }

    public void UpdateChosenRecipe(CraftingRecipe recipe)
    {
        // Aktualizuje wybrany przepis oraz interfejs
        _chosenRecipe = recipe;
        DisplayItemPreview(_chosenRecipe.CraftedItem);
        PopulateIngredientList(); // Wyświetla listę składników

        _chanceValueText.text = _chosenRecipe.SuccessChance.ToString("0.0") + "%"; // Wyświetla szansę na sukces
        ActivateConstantUI(); // Aktywuje odpowiednie elementy UI
        _craftButton.onClick.RemoveAllListeners();
        _craftButton.onClick.AddListener(() => CraftItem()); // Dodaje akcję do przycisku craftingu
    }

    private void CraftItem()
    {
        // Rozpoczyna proces craftingu
        int successfulCrafts = 0; // Liczba udanych prób craftingu
        int failedCrafts = 0; // Liczba nieudanych prób craftingu
        List<string> failureReasons = new List<string>(); // Lista powodów niepowodzeń

        // Pętla do wykonywania craftingu dla każdej żądanej ilości
        for (int i = 0; i < _desiredCraftAmount; i++)
        {
            var (success, reason) = _craftingSystem.TryToCraftAnItem(_chosenRecipe);
            if (success)
            {
                successfulCrafts++; // Zwiększ liczbę udanych craftów
            }
            else
            {
                failedCrafts++; // Zwiększ liczbę nieudanych craftów
                failureReasons.Add(reason);  // Dodaj powód niepowodzenia do listy
            }
        }
        
        // string.Join(", ", failureReasons.Distinct()) - Łączy wszystkie unikalne powody niepowodzeń w jeden ciąg znaków, oddzielając je przecinkami. 
        // Distinct() - Usuwa duplikaty powodów z listy failureReasons, aby każdy powód wystąpił tylko raz.
        
        // Aktualizacja interfejsu użytkownika w zależności od wyniku craftingu
        if (successfulCrafts == _desiredCraftAmount)
        {
            Debug.Log("Crafting successful!");
            _craftingText.text = $"Crafting {_desiredCraftAmount}x {_chosenRecipe.CraftedItem.ItemName} zakończony sukcesem.";
        }
        else if (successfulCrafts > 0)
        {
            Debug.Log("Partial crafting success.");
            _craftingText.text = $"Crafting {_desiredCraftAmount}x {_chosenRecipe.CraftedItem.ItemName} częściowo udany. " +
                                 $"{successfulCrafts} udanych, {failedCrafts} nieudanych. Powody: <color=red>{string.Join(", ", failureReasons.Distinct())}</color>";
        }
        else
        {
            Debug.Log("Crafting failed.");
            _craftingText.text = $"Crafting {_desiredCraftAmount}x {_chosenRecipe.CraftedItem.ItemName} nieudany. Powody: <color=red>{string.Join(", ", failureReasons.Distinct())}</color>";
        }

        _itemImage.sprite = _chosenRecipe.CraftedItem.Icon; // Ustaw img przedmiotu w oknie
        _craftingOutputWindow.SetActive(true); // Wyświetl okno wyników craftingu
    }


    private void CloseCraftingOutputWindow()
    {
        // Zamyka okno wyników
        _craftingOutputWindow.SetActive(false);
    }

    private void PopulateIngredientList()
    {
        // Wyświetla listę składników potrzebnych do craftingu
        ClearSlots(_ingredientGrid); // Czyści sloty składników
        ResetCraftAmount(); // Resetuje ilość craftowanych przedmiotów
        InitializeCraftAmountButtons(); // Inicjalizuje przyciski zmiany ilości

        foreach (var ingredient in _chosenRecipe.Ingredients)
        {
            var ingredientSlot = Instantiate(_ingredientSlotPrefab, _ingredientGrid);
            ingredientSlot.Init(ingredient.ItemRequired, ingredient.AmountRequired);
            _ingredientSlots.Add(ingredientSlot); // Dodaje sloty do listy
        }
    }

    private void InitializeCraftAmountButtons()
    {
        if (!_decreaseCraftAmountButton.IsActive())
        {
            _decreaseCraftAmountButton.gameObject.SetActive(true);
            _decreaseCraftAmountButton.onClick.AddListener(() => ChangeCraftAmount(-1));
        }

        if (!_increaseCraftAmountButton.IsActive())
        {
            _increaseCraftAmountButton.gameObject.SetActive(true);
            _increaseCraftAmountButton.onClick.AddListener(() => ChangeCraftAmount(1));
        }
    }
    
    //odsiwez sloty skladnikow

    private void RefreshIngredientDisplay()
    {
        foreach (var ingredient in _chosenRecipe.Ingredients)
        {
            foreach (var slot in _ingredientSlots)
            {
                if (slot.AssignedData == ingredient.ItemRequired)
                {
                    slot.UpdateRequiredAmount(ingredient.AmountRequired * _desiredCraftAmount);
                }
            }
        }
    }
}
