using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseBotController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private CharacterAnimatorController _animatorController;

    protected IReactiveProperty<BotState> BotStateProperty => _botStateProperty;
    private readonly ReactiveProperty<BotState> _botStateProperty = new();
    
    private IDisposable _intervalDisposable;

    private void OnEnable()
    {
        _intervalDisposable = Observable.Interval(TimeSpan.FromSeconds(.4f)).Subscribe(_ =>
        {
            UpdateBotState();
            _animatorController.UpdateAnimations(_botStateProperty.Value);
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
}