using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Inventory System/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Składniki potrzebne do stworzenia przedmiotu")]
    [SerializeField] private List<CraftingIngredient> _ingredients;  // Lista składników potrzebnych do wykonania przepisu

    [Header("Przedmiot, który zostanie wytworzony")]
    [SerializeField] private ItemData _craftedItem;  // Przedmiot, który otrzymamy po wykonaniu przepisu

    [Header("Ilość przedmiotów do wytworzenia")]
    [SerializeField, Min(1)] private int _craftedAmount = 1;  // Ile sztuk przedmiotu ma być wytworzonych

    [Header("Szansa na sukces wytworzenia przedmiotu (%)")]
    [SerializeField, Range(0, 100)] private float _successChance = 100f;  // Szansa na sukces przy wytwarzaniu przedmiotu

    [Header("Identyfikatory eventów sukcesu (można dodać kilka)")]
    [SerializeField] private List<string> successEventIdentifiers = new List<string>();  // Lista eventów, które mają być wywołane przy sukcesie

    [Header("Identyfikatory eventów niepowodzenia (można dodać kilka)")]
    [SerializeField] private List<string> failureEventIdentifiers = new List<string>();  // Lista eventów, które mają być wywołane przy niepowodzeniu
    public List<CraftingIngredient> Ingredients => _ingredients;

    public ItemData CraftedItem => _craftedItem;
    public int CraftedAmount => _craftedAmount;
    
    public float SuccessChance => _successChance;
    
    // trigger jezeli crafting skoncz sie scuckesem
    public void TriggerSuccess()
    {
        foreach (var identifier in successEventIdentifiers)
        {
            CraftingEventManager.Instance.TriggerCraftingSuccess(identifier, this);
        }
    }

    // gdy crafting się nie powiedzie
    public void TriggerFailure()
    {
        foreach (var identifier in failureEventIdentifiers)
        {
            CraftingEventManager.Instance.TriggerCraftingFailure(identifier, this);
        }
    }
}

// skladnik potrzebny do recipe craftingu


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