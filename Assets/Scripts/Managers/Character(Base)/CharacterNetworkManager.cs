using System;
using Unity.Netcode;
using UnityEngine;

public class CharacterNetworkManager : NetworkBehaviour
{
    private CharacterManager _character;

    protected void Awake()
    {
        _character = GetComponent<CharacterManager>();
    }

    [Header("Pozycja/Rotacja")] public NetworkVariable<Vector3> networkPosition = new(Vector3.zero,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<Quaternion> networkRotation = new(Quaternion.identity,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotationSmoothTime = 0.1f;

    [Header("Animator")] [SerializeField] public NetworkVariable<float> horizontalNetMovement =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] public NetworkVariable<float> verticalNetMovement =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] public NetworkVariable<float> moveNetAmount =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flagi")] public NetworkVariable<bool> isSprinting = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    
    public NetworkVariable<bool> isJumping = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    
    // ServerRpc wywoływane przez klienta do serwera
    [ServerRpc]
    public void NotifyServerOfActionAnimationServerRpc(ulong clientId, string animationId, bool applyRootMotion)
    {
        // Jeśli to serwer, wyślij do klientów
        if (IsServer)
        {
            PlayActionAnimationOnClientRpc(clientId, animationId, applyRootMotion);
        }
    }

// ClientRpc wysyłane do wszystkich klientów przez serwer
    [ClientRpc]
    public void PlayActionAnimationOnClientRpc(ulong clientId, string animationId, bool applyRootMotion)
    {
        // Sprawdza, czy to nie ten sam klient, żeby nie odtwarzać animacji dwa razy
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            ExecuteActionAnimation(animationId, applyRootMotion);
        }
    }

// Odtwórz animację, zastosuj root motion jeśli trzeba
    private void ExecuteActionAnimation(string animationId, bool applyRootMotion)
    {
        _character.applyRootMotion = applyRootMotion; // Ustawiamy root motion
        _character.animator.CrossFade(animationId, 0.2f); // Płynne przejście do nowej animacji
    }

}