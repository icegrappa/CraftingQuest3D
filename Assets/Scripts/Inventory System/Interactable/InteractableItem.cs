using System;
using System.Collections;
using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData; // itemdata dla reprzetnacji itemu w swiecie 
    [SerializeField] private float interactionRange = 2f; // zakres interakcji
    [SerializeField] private float angleThreshold = 45f; // kat pomiedzy puntkeim interakcji a graczem
    [SerializeField] public float checkInterval = 0.1f; // Częstotliwosc sprawdzania kolizji
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private string interactionMessage;

    private readonly Collider[] hitColliders = new Collider[10];
    private Transform playerTransform;
    private GlobalInputManager _globalInput;

    public float CurrentDistanceToPlayer { get; private set; }
    
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
            InventoryUIManager.instance.ShowInteractionPrompt(itemData, 1, interactionMessage);  //
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
            playerTransform = null;
            InteractableManager.RemoveInteractable(this); //
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
        var pointer = playerTransform.GetComponent<GlobalEventHandler>().GetTargetTransform();

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

    
    public void Interact(Transform interactingTransform)
    {
        Debug.Log("Interakcja z itemem");
        InventoryUIManager.instance.HideInteractionPrompt(); // Ukrywamy prompt po interakcji

        var inventory = interactingTransform.GetComponent<InventoryContainer>();
        var globalHandler = interactingTransform.GetComponent<GlobalEventHandler>();
        
        if (inventory != null)

            globalHandler.InvokeGlobalEvent();
            if (inventory.InventorySystem.AddItem(itemData, 1))
              
            if (gameObject.transform.parent != null)
            {
                // If the GameObject has a parent, destroy the parent GameObject
                Destroy(gameObject.transform.parent.gameObject);
            }
            else
            {
                // If the GameObject has no parent, destroy the current GameObject
                Destroy(gameObject);
            }

    }

  
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}