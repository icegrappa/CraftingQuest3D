using System;
using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData; // itemdata dla reprzetnacji itemu w swiecie 
    [SerializeField] private float interactionRange = 2f; // zakres interakcji
    [SerializeField] private float angleThreshold = 45f; // kat pomiedzy puntkeim interakcji a graczem
    [SerializeField] public float checkInterval = 0.1f; // Częstotliwosc sprawdzania kolizji
    [SerializeField] private Canvas interactionCanvas; 
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask interactableLayer;

    private readonly Collider[] hitColliders = new Collider[10];
    private Transform playerTransform;
    private GlobalInputManager _globalInput;

    public float CurrentDistanceToPlayer { get; private set; }

    private void Awake()
    {
        if (interactionCanvas != null) HideInteractionPrompt();
    }

    private void Start()
    {
        _globalInput = GlobalInputManager.instance;
        
        StartCoroutine(CheckForPlayerCoroutine());
    }
    
    private IEnumerator CheckForPlayerCoroutine()
    {
        while (true)
        {
            if (playerTransform == null)
            {
                FindPlayer();
            }

            yield return new WaitForSeconds(checkInterval); // Używamy wartości z Inspectora
        }
    }

    private void Update()
    {
        if (_globalInput == null || playerTransform == null) return; // jezlei nie ma ggracza 
      
        // Aktualizujemy odległość do gracza
        CurrentDistanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Sprawdzamy, czy obiekt ma mozliwosci nterakcji
        if (IsInteractable(playerTransform))
        {
            ShowInteractionPrompt(); //
            InteractableManager.AddInteractable(this); 

            // Sprawdzamy, czy użytkownik wywołał interakcję
            if (_globalInput.interactInput)
            {
                Interact(playerTransform); 
                _globalInput.interactInput = false; // Resetujemy stan interakcji
                InteractableManager.RemoveInteractable(this); //
            }
        }
        else
        {
            HideInteractionPrompt();
            playerTransform = null;
            InteractableManager.RemoveInteractable(this); //
        }

        // Resetujemy stan interakcji jeśli nie ma żadnych interaktywnych obiektów w pobliżu
        if (!InteractableManager.HasInteractables())
        {
            _globalInput.interactInput = false;
        }
    }




    private void FindPlayer()
    {
        //https://discussions.unity.com/t/overlapsphere-and-overlapspherenonalloc/656660
        // Szukamy gracza w zasięgu interakcji
        var numColliders = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            hitColliders,
            playerLayer
        );

        for (var i = 0; i < numColliders; i++)
            if (hitColliders[i] != null)
            {
                playerTransform = hitColliders[i].transform; // Ustawiamy referencję do gracza
                break;
            }
    }

    private bool IsClosestInteractable()
    {
        // Sprawdzamy czy ten obiekt jest najbliższym interaktywnym obiektem
        var numColliders = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            hitColliders,
            interactableLayer
        );

        for (var i = 0; i < numColliders; i++)
        {
            var other = hitColliders[i]?.GetComponent<InteractableItem>();
            if (other != null && other != this && other.CurrentDistanceToPlayer < CurrentDistanceToPlayer) return false; // Znaleziono bliższy obiekt
        }

        return true; // Ten obiekt jest najbliższy
    }

    private bool IsInteractable(Transform playerTransform)
    {
        return IsInRange(playerTransform) && !IsObstructed(playerTransform) && IsFacing(playerTransform);
    }

    public bool IsInRange(Transform playerTransform)
    {
        return CurrentDistanceToPlayer <= interactionRange;
    }

    public bool IsObstructed(Transform playerTransform)
    {
        var pointer = playerTransform.GetComponentInChildren<PositionPointer>();

        if (pointer != null)
        {
            RaycastHit hit;
            if (Physics.Linecast(pointer.transform.position, transform.position, out hit))
                return hit.transform != transform;
            return false;
        }

        return true;
    }

    public bool IsFacing(Transform playerTransform, float angleThreshold = 45f)
    {
        var directionToTarget = (transform.position - playerTransform.position).normalized;
        var angle = Vector3.Angle(playerTransform.forward, directionToTarget);
        return angle <= angleThreshold;
    }

    public void ShowInteractionPrompt()
    {
        if (interactionCanvas != null) interactionCanvas.gameObject.SetActive(true);
    }

    
    public void Interact(Transform interactingTransform)
    {
        Debug.Log("Interacting with the item");
        HideInteractionPrompt(); // Ukrywamy prompt po interakcji

        var inventory = interactingTransform.GetComponent<InventoryContainer>();
        if (inventory != null)

            if (inventory.InventorySystem.AddItem(itemData, 1))
                Destroy(gameObject.transform.root.gameObject); // Dodajemy item do inventory, jeśli się udało, niszczymy obiekt
    }

    public void HideInteractionPrompt()
    {
        if (interactionCanvas != null) interactionCanvas.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}