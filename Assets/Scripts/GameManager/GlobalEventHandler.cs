using UnityEngine;
using UnityEngine.Events;

public class GlobalEventHandler : MonoBehaviour
{
    [Header("Serialized Event")]
    [SerializeField] private UnityEvent onGlobalEvent; // globalny event

    [Header("Serialized Transform")]
    [SerializeField] private Transform targetTransform; //

    // Wywołaj globalny event
    public void InvokeGlobalEvent()
    {
        onGlobalEvent?.Invoke(); // 
    }
    
    public void ClearGlobalListeners()
    {
        onGlobalEvent.RemoveAllListeners(); // Wyczyść listę
    }
    
    public Transform GetTargetTransform()
    {
        return targetTransform; 
    }
}