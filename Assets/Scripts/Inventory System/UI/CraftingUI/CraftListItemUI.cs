using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftListItemUI : MonoBehaviour
{
    [SerializeField] private Image _recipeSprite;
    [SerializeField] private TextMeshProUGUI _recipeName;

    [SerializeField] private Button _button;

    private CraftingUISystem _parentUISystem;
    private CraftingRecipe _recipe;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void Init(CraftingRecipe recipe, CraftingUISystem parentUISystem)
    {
        _parentUISystem = parentUISystem;
        _recipe = recipe;

        _recipeSprite.sprite = _recipe.CraftedItem.Icon;
        _recipeName.text = _recipe.CraftedItem.ItemName;
    }

    public void OnButtonClicked()
    {
        if (_parentUISystem == null) return;

        _parentUISystem.UpdateChosenRecipe(_recipe);
    }
}