using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUISystem : MonoBehaviour
{
    [Header("Interaction UI")]
    [SerializeField] private GameObject itemPopup; 
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemAmountText; 
    [SerializeField] private Image itemIcon; 
    [SerializeField] private TextMeshProUGUI interactionText; 
    
 
    public bool IsItemPopupActive => itemPopup.activeSelf;

    
        //pokaz interakcje
    public void ShowItemPopup(ItemData itemData, int amount, string interactionMessage)
    {
        
        itemNameText.text = itemData.ItemName;

    
        itemAmountText.text = $"Amount: {amount}";

       
        itemIcon.sprite = itemData.Icon;
        itemIcon.enabled = true; 

      
        interactionText.text = interactionMessage;

        itemPopup.SetActive(true);
    }

    //
    public void ClearItemPopup()
    {
        
        itemNameText.text = string.Empty;
        itemAmountText.text = string.Empty;
        interactionText.text = string.Empty;

      
        itemIcon.sprite = null;
        itemIcon.enabled = false;

       
        itemPopup.SetActive(false);
    }
}