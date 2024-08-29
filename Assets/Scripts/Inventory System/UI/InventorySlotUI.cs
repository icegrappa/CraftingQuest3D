using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private InventorySlot assignedInventorySlot;

    private Button _button;

    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryUISystem ParentUISystem { get; private set; }

    private void Awake()
    {
        ClearSlot();

        _button = GetComponent<Button>();
        _button?.onClick.AddListener(OnUISlotClickedAction);

        ParentUISystem = transform.parent.GetComponent<InventoryUISystem>();
    }

    // metoda po kliknieciu slotu
    public void OnUISlotClickedAction()
    {
        ParentUISystem?.SlotClicked(this);
    }

    // inicjalizacja slotu 
    public void Initialize(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);
    }
    

    // Aktualizacja UI slotu na podstawie danych slotu inventory
    public void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.Icon;
            itemSprite.color = Color.white;
            if (slot.StackSize > 1) itemCount.text = slot.StackSize.ToString();
            else itemCount.text = string.Empty;
        }

        else
        {
            {
                ClearSlot();
            }
        }
    }

    // Aktualizacja UI na  slotu inv/ tego ktory juz jest przypisany
    public void UpdateUISlot()
    {
        if (assignedInventorySlot != null)
            UpdateUISlot(assignedInventorySlot);
    }


    // Czyszczenie elementów UI slotu
    public void ClearSlot()
    {
        assignedInventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = string.Empty;
    }
}