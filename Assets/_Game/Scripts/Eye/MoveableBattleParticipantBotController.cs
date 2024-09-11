using Random = UnityEngine.Random;
using UnityEngine;
using System;
using UniRx;

public enum BotType
{
    Soldier,
    Juggernaut,
    Boss
}

namespace Bot.BotController
{
    public class MoveableBattleParticipantBotController : MoveableBattleParticipantBaseController
    {
        [SerializeField] protected BotBattleParticipant battleParticipant;
        [SerializeField] private BotType type;
        [Space] 
        [SerializeField] private BotBehaviourModel _model;

        private ReactiveProperty<BotState> _state = new(BotState.Idle);

        private IBotMonoBehaviour _botBehaviour;
        private BehaviourControllerBase _behaviourController;

        private IMoveableRigidbody _moveableRigidbody;

        private IBattleParticipantParameters _closestBattleParticipantElement;
        //

        private IDisposable _closestElementDisposable;
        private IDisposable _behaviourUpdateDisposable;

        private ITransform _closestEnemyTransform;
        private Vector3 _currentMoveDirection;
        

        internal void Activate(BotType botType, BotBehaviourModel model)
        {
            type = botType;
            _model = model;

            _speed.Value = _model.Speed;

            switch (type)
            {
                case BotType.Soldier:
                    _botBehaviour = new BotMiddleBehaviour(model);
                    _behaviourController = new BehaviourSoliderController();
                    break;
                case BotType.Juggernaut:
                    _botBehaviour = new BotAggressiveBehaviour(model);
                    _behaviourController = new BehaviourJuggernautController();
                    break;
                default:
                    _botBehaviour = new BotMiddleBehaviour(model);
                    _behaviourController = new BehaviourSoliderController();
                    break;
            }
        }

        protected void Start()
        {
            _closestElementDisposable = Observable.Interval(
                TimeSpan.FromSeconds(Random.Range(0.5f, 1f))).Subscribe(_ =>
            {
                if (battleParticipant.GetClosestElement(out var result))
                {
                    _closestBattleParticipantElement = result;
                    _closestEnemyTransform = result.BotTransform;
                }
                
            }).AddTo(this);

            _behaviourUpdateDisposable = Observable.Interval(
                    TimeSpan.FromSeconds(Random.Range(0.5f, 1.5f)))
                .Subscribe(_ => { UpdateBehaviourState(); })
                .AddTo(this);

            _state.Subscribe(state =>
            {
                if (state == BotState.Idle)
                {
                    
                }
                else
                {
                    
                }
                
            }).AddTo(this);
        }

        private void UpdateBehaviourState()
        {
            _state.Value = _botBehaviour.BotBehaviourUpdate(this,
                _closestBattleParticipantElement);

            var model = _behaviourController.SetBehaviourState(_state.Value,
                this,
                _closestEnemyTransform);

            moveDirection = model.MoveDirection;
        }
        
        public void PoolActivate()
        {
            gameObject.SetActive(true);
        }

        public void PoolDeactivate()
        {
            _behaviourUpdateDisposable.Dispose();
            _closestElementDisposable.Dispose();
            gameObject.SetActive(false);
        }

        public void PoolDestroy()
        {
            Destroy(gameObject);
        }
    }
}