using Random = UnityEngine.Random;
using UnityEngine;
using System;
using Pooling;
using UniRx;

public enum BotType
{
    Soldier,
    Juggernaut,
}

namespace Bot.BotController
{
    public class EyeBotController : EyeBaseController, IPoolingMono
    {
        [SerializeField] private BotType type;
        [SerializeField] private BotBattleParticipant _battleParticipant;
        [Space] [SerializeField] private BotBehaviourModel _model;

        private ReactiveProperty<BotState> _state = new(BotState.Idle);
        //
        public MonoBehaviour PoolMonoObj => this;

        //
        private IBotMonoBehaviour _botBehaviour;
        private BehaviourControllerBase _behaviourController;

        private IMoveableRigidbody _moveableRigidbody;

        private IEyeParameters _closestEyeElement;
        //

        private IDisposable _closestElementDisposable;
        private IDisposable _behaviourUpdateDisposable;

        private ITransform _closestEnemyTransform;
        private Vector3 _currentMoveDirection;

        private void Awake()
        {
            _moveableRigidbody = new MoveWithRbAddForce();
        }

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

        protected override void Start()
        {
            base.Start();
            
            _closestElementDisposable = Observable.Interval(
                TimeSpan.FromSeconds(Random.Range(0.5f, 1f))).Subscribe(_ =>
            {
                if (_battleParticipant.GetClosestElement(out var result))
                {
                    _closestEyeElement = result;
                    _closestEnemyTransform = result.EyeTransform;
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
                    MoveBalanceStop();
                }
                else
                {
                    MoveBalanceStart();
                }
            }).AddTo(this);
        }

        private void UpdateBehaviourState()
        {
            _state.Value = _botBehaviour.BotBehaviourUpdate(this,
                _closestEyeElement);

            var model = _behaviourController.SetBehaviourState(_state.Value,
                this,
                _closestEnemyTransform);
            
            
            
            moveDirection = model.MoveDirection;

            if (_closestEnemyTransform != null)
            {
                Debug.DrawLine(IPosition, _closestEnemyTransform.IPosition, Color.red);
                Debug.DrawLine(IPosition, _closestEnemyTransform.IPosition + Vector3.up * 10, Color.red);
                Debug.DrawLine(_closestEnemyTransform.IPosition, _closestEnemyTransform.IPosition + Vector3.up * 10,
                    Color.red);
            }
        }

        protected override void Move()
        {
            _currentMoveDirection = Vector3.Lerp(
                _currentMoveDirection,
                moveDirection,
                Time.deltaTime);

            _moveableRigidbody.Move(Rb,
                _currentMoveDirection, 0.5f);

            var dir = Vector3.ProjectOnPlane(_currentMoveDirection * 15, Vector3.up);
            
            Debug.DrawRay(Position, dir, Color.cyan);
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