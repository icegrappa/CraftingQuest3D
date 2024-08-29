using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] private List<CraftingIngredient> _ingredients;
    [SerializeField] private ItemData _craftedItem;
    [SerializeField, Min(1)] private int _craftedAmount = 1;

    public List<CraftingIngredient> Ingredients => _ingredients;

    public ItemData CraftedItem => _craftedItem;
    public int CraftedAmount => _craftedAmount;
}

[System.Serializable]
public struct CraftingIngredient
{
    public ItemData ItemRequired;
    public int AmountRequired;

    public CraftingIngredient(ItemData itemRequired, int amountRequired)
    {
        ItemRequired = itemRequired;
        AmountRequired = amountRequired;
    }
}