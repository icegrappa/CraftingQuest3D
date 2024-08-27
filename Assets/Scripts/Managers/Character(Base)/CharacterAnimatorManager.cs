using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    protected static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    protected static readonly int VerticalHash = Animator.StringToHash("Vertical");

    private CharacterManager _character;

    private float vertical;
    private float horizontal;

    protected virtual void Awake()
    {
        _character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementValues(float horizontalValue, float verticalValue)
    {
        _character.animator.SetFloat(HorizontalHash, horizontalValue, 0.1f, Time.deltaTime);
        _character.animator.SetFloat(VerticalHash, verticalValue, 0.1f, Time.deltaTime);
    }

    public virtual void PlayActionAnimation(string targetAnimation, bool isPerformingAction,
        bool applyRootMotion = true)
    {
        _character.animator.applyRootMotion = applyRootMotion;
        _character.animator.CrossFade(targetAnimation,0.2f);
    }
}