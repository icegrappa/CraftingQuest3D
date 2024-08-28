using UnityEngine;
using Unity.Netcode;
public class CharacterAnimatorManager : MonoBehaviour
{
    private CharacterManager _character;

    private int _vertical;
    private int _horizontal;
    
    [HideInInspector]public int _isGrounded;
    [HideInInspector]public int _inAirTimer;

    protected virtual void Awake()
    {
        _character = GetComponent<CharacterManager>();
        
        _vertical = Animator.StringToHash("Vertical");
        _isGrounded = Animator.StringToHash("isGrounded");
        _horizontal = Animator.StringToHash("Horizontal");
        _inAirTimer = Animator.StringToHash("InAirTimer");
    }

    public void UpdateAnimatorMovementValues(float horizontalValue, float verticalValue, bool isSprinting)
    {
        if (isSprinting)
        {
            verticalValue = 2;
        }
        
        _character.animator.SetFloat(_horizontal, horizontalValue, 0.1f, Time.deltaTime);
        _character.animator.SetFloat(_vertical, verticalValue, 0.1f, Time.deltaTime);
    }

    public virtual void PlayActionAnimation(string targetAnimation, bool isPerformingAction,
        bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        _character.animator.applyRootMotion = applyRootMotion;
        _character.animator.CrossFade(targetAnimation,0.2f);
        
        // ta flaga zatrzyma przed wykonaniem akcji podczas tej animacji
        _character.isPerformingAction = isPerformingAction;
        // czy postac moze zminiac rotacje podczas animacji?
        _character.canRotate = canRotate;
        // czy moze sie ruszac?
        _character.canMove = canMove;
        
        _character.characterNetworkManager.NotifyServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }
}