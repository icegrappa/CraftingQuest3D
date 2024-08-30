using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemCount;

    public ItemData AssignedData { get; private set; }

    public void Init(ItemData data, int amount)
    {
        AssignedData = data;
        _itemSprite.preserveAspect = true;
        _itemSprite.sprite = data.Icon;
        _itemSprite.color = Color.white;
        UpdateRequiredAmount(amount);
    }

    public void UpdateRequiredAmount(int amount, bool playerHasRequiredItems = true)
    {
        _itemCount.text = amount.ToString();

        if (!playerHasRequiredItems)
            _itemCount.color = Color.red;
    }
}