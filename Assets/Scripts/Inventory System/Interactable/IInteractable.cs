using UnityEngine;


// interfejs dla wszyskich kolejnych mono rozszerzalny dla roznego typu interakcjii
public interface IInteractable
{
    // VCzy gracz znajduje się w zasięgu interakcji
    bool IsInRange(Transform playerTransform);

    // czy nie ma przeszkód między graczem a obiektem
    bool IsObstructed(Transform playerTransform);

    // Gracz zwrocony ?
    bool IsFacing(Transform playerTransform, float angleThreshold = 45f);

    // Metoda, która jest wywoływana, gdy gracz spełnia warunki interakcji // np aktywuj UI

    // Metoda do faktycznego przeprowadzenia interakcji / podenies przedmiot
    void Interact(Transform interactingTransform);
}