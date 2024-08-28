using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Item Data")]
public class ItemData : ScriptableObject
{
    // unikalny ID przedmiotu do identyfikacji
    [SerializeField] public int ID;

    // nazwa przedmiotu która wyświetla się w ekwipunku
    [SerializeField] public string ItemName;

    // opis przedmiotu
    [TextArea(4, 4)] [SerializeField] private string itemInfo;

    public string ItemInfo
    {
        get => itemInfo;
        private set => itemInfo = value;
    }

    // ikona która reprezentuje przedmiot w UI
    public Sprite Icon;

    // maksymalna liczba przedmiotów które mogą być zgrupowane razem
    [SerializeField] public int MaxStackSize;

    // metoda inicjalizująca dane przedmiotu
    public void Initialize(int id, string itemName, string itemInfo, Sprite icon, int maxStackSize)
    {
        ID = id;
        ItemName = itemName;
        ItemInfo = itemInfo;
        Icon = icon;
        MaxStackSize = maxStackSize;
    }
}