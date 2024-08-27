using Unity.Netcode;
using UnityEngine;

public class CharacterNetworkManager : NetworkBehaviour
{
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
}