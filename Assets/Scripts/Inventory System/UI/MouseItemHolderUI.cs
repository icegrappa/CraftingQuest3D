using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseItemHolderUI : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount; // Liczba przedmiotów na myszy
    public InventorySlot AssignedInventorySlot; // Slot przypisany do przedmiotu na myszy
    [SerializeField] private GameObject sliderParent;
    [SerializeField] private Slider holdSlider;

    private readonly float holdTime = 2f; // Czas potrzebny do wypełnienia slidera 
    private float holdProgress; // Aktualny postęp wypełnienia slidera

    private void Awake()
    {
        // Inicjalizacja stanu początkowego UI na myszy
        ItemSprite.color = Color.clear;
        ItemCount.text = string.Empty;
        sliderParent.SetActive(false); // Wyłącz slider na start
        holdSlider.value = 0;
    }

    public void UpdateMouseHolderSlot(InventorySlot inventorySlot)
    {
        // Aktualizacja slotu przedmiotu na myszy
        AssignedInventorySlot.AssignItem(inventorySlot);
        ItemSprite.sprite = inventorySlot.ItemData.Icon;
        ItemCount.text = inventorySlot.StackSize.ToString();
        ItemSprite.color = Color.white; // Ustawienie widoczności ikonki
    }

    public void ClearMouseHolderAssignedSlot()
    {
        // Czyszczenie slotu na myszy
        if (AssignedInventorySlot != null)
            AssignedInventorySlot.ClearSlot();
        ItemCount.text = string.Empty;
        ItemSprite.color = Color.clear; // Ukrycie ikonki
        ItemSprite.sprite = null;
    }

    private void Update()
    {
        if (AssignedInventorySlot.ItemData != null)
        {
            // Aktualizacja pozycji UI na myszy
            transform.position = Mouse.current.position.ReadValue();

            // Obiekty do ignorowania podczas sprawdzania UI
            GameObject[] objectsToIgnore = { sliderParent, holdSlider.gameObject };

            // Sprawdzenie, czy trzymany jest przycisk myszy i czy nie najeżdżamy na element UI
            var isHolding = GlobalInputManager.instance.deleteItemUIInput &&
                            !IsPointerOverUIObject(objectsToIgnore, out var hoveredObject);

            if (isHolding)
            {
                if (!sliderParent.activeSelf)
                    sliderParent.SetActive(true); // Włączamy slider, jeśli jeszcze nie jest aktywny

                // Zwiększanie postępu slidera
                holdProgress += Time.deltaTime / holdTime * 100f;
                holdSlider.value = holdProgress;

                if (holdProgress >= 100f)
                {
                    // Pobranie pozycji świata dla instancji przedmiotu
                    var mousePosition = GameManager.instance.GetMouseWorldPosition();
                    var stackSize = AssignedInventorySlot.StackSize;

                    // Sprawdzenie, czy instancjować jeden przedmiot, czy wiele
                    if (stackSize > 1)
                        // Instancjowanie wielu przedmiotów w losowych miejscach
                        GameManager.instance.InstantiateItemInWorld(mousePosition,
                            AssignedInventorySlot.ItemData.itemPrefab, true, stackSize, 2f);
                    else
                        // Instancjowanie pojedynczego przedmiotu
                        GameManager.instance.InstantiateItemInWorld(mousePosition,
                            AssignedInventorySlot.ItemData.itemPrefab);

                    ClearMouseHolderAssignedSlot(); // Czyszczenie slotu na myszy
                    ResetSlider(); // Resetowanie slidera
                }
            }
            else if (holdProgress > 0f)
            {
                // Zmniejszanie postępu slidera
                holdProgress -= Time.deltaTime / holdTime * 100f;
                holdProgress = Mathf.Clamp(holdProgress, 0, 100f);
                holdSlider.value = holdProgress;

                if (holdProgress <= 0f) sliderParent.SetActive(false); // Wyłączenie slidera, gdy postęp osiągnie 0
            }
        }
        else
        {
            ResetSlider(); // Resetowanie slidera, jeśli brak przedmiotu na myszy
        }
    }

    private void ResetSlider()
    {
        // Resetowanie stanu slidera
        holdProgress = 0f;
        holdSlider.value = 0f;
        sliderParent.SetActive(false);
    }

    //https://stackoverflow.com/questions/39150165/how-do-i-find-which-object-is-eventsystem-current-ispointerovergameobject-detect
    public static bool IsPointerOverUIObject(GameObject[] objectsToIgnore, out GameObject hoveredObject)
    {
        // Sprawdzanie, czy kursor znajduje się nad elementem UI, ignorując określone obiekty
        hoveredObject = null;
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (var result in results)
        {
            // Sprawdzanie, czy obiekt znajduje się na liście ignorowanych
            var shouldIgnore = false;
            foreach (var obj in objectsToIgnore)
                if (result.gameObject == obj || result.gameObject.transform.IsChildOf(obj.transform))
                {
                    shouldIgnore = true;
                    break;
                }

            if (!shouldIgnore)
            {
                hoveredObject = result.gameObject;
                return true; // Zwracamy true, jeśli kursor jest nad elementem UI, którego nie ignorujemy
            }
        }

        return false;
    }
}