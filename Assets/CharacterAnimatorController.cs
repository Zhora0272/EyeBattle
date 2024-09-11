using UnityEngine;

public class CharacterAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int RunHash = Animator.StringToHash("Run");
    
    internal void UpdateAnimations(BotState state)
    {
        _animator.SetBool(IdleHash, state == BotState.Idle);
        _animator.SetBool(RunHash, state == BotState.Run);
    }
}