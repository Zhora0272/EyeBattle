using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseBotController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    protected IReactiveProperty<BotState> BotStateProperty => _botStateProperty;
    private readonly ReactiveProperty<BotState> _botStateProperty = new();

    #region AnimationStringHash

    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int RunHash = Animator.StringToHash("Run");

    #endregion

    private IDisposable _intervalDisposable;
    
    private void OnEnable()
    {
        _intervalDisposable = Observable.Interval(TimeSpan.FromSeconds(.4f)).Subscribe(_ =>
        {
            UpdateBotState();
            UpdateAnimations();
            
        }).AddTo(this);
    }

    private void OnDisable()
    {
        _intervalDisposable?.Dispose();
    }

    private void UpdateBotState()
    {
        if (_agent.velocity.magnitude < .3f)
        {
            _botStateProperty.Value = BotState.Idle;
        }
        else
        {
            _botStateProperty.Value = BotState.Run;
        }
    }

    private void UpdateAnimations()
    {
        _animator.SetBool(IdleHash, _botStateProperty.Value == BotState.Idle);
        _animator.SetBool(RunHash, _botStateProperty.Value == BotState.Run);
    }
}