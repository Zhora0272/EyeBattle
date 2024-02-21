using _Game.Scripts.Utility;
using UnityEngine;

public class BehaviourControllerModel
{
    public Vector3 MoveDirection;

    public float SpeedPercentage
    {
        get => _speedPercentage;
        set
        {
            if (value > 1)
            {
                _speedPercentage = 1;
            }
            else if (value < 0)
            {
                _speedPercentage = 0;
            }
        }
    }
    private float _speedPercentage;
    
    public float RotationSpeedPercentage
    {
        get => _rotationSpeedPercentage;
        set
        {
            if (value > 1)
            {
                _rotationSpeedPercentage = 1;
            }
            else if (value < 0)
            {
                _rotationSpeedPercentage = 0;
            }
        }
    }
    private float _rotationSpeedPercentage;
}

public abstract class BehaviourControllerBase
{
    protected readonly BehaviourControllerModel model;

    protected BehaviourControllerBase()
    {
        model = new BehaviourControllerModel();
    }

    public abstract BehaviourControllerModel SetBehaviourState
    (
        BotState state,
        ITransform transform,
        ITransform closestEnemy
    );
}

public class BehaviourJuggernautController : BehaviourControllerBase
{
    private float _attackTime;
    private float _attackInterval = 4;
    public override BehaviourControllerModel SetBehaviourState
    (
        BotState state,
        ITransform transform,
        ITransform closestEnemy
    )
    {
        switch (state)
        {
            case BotState.RandomWalk:
                model.MoveDirection = (HelperMath.GetRandomPosition(-1f, 1f) * 10).normalized;
                break;
            case BotState.Attack:
                if (Time.time - _attackTime > _attackInterval * 0.8f)
                {
                    model.SpeedPercentage = 0;
                }
                if (Time.time - _attackTime > _attackInterval)
                {
                    model.SpeedPercentage = 1;
                    _attackTime = Time.time;
                    model.MoveDirection = closestEnemy.IPosition - transform.IPosition;
                }
                break;
        }

        return model;
    }

    public BehaviourJuggernautController() : base()
    {
    }
}

public class BehaviourSoliderController : BehaviourControllerBase
{
    public override BehaviourControllerModel SetBehaviourState
    (
        BotState state,
        ITransform transform,
        ITransform closestEnemy
    )
    {
        var moveDirection = Vector3.zero; 
        
        switch (state)
        {
            case BotState.RandomWalk:
                moveDirection = (HelperMath.GetRandomPosition(-1f, 1f) * 10).normalized;
                break;
            case BotState.Idle:
                moveDirection = Vector3.zero;
                break;
            case BotState.GoAwayFromEnemy:
                moveDirection = transform.IPosition - closestEnemy.IPosition;
                break;
            case BotState.Attack:
                moveDirection = closestEnemy.IPosition - transform.IPosition;
                break;
        }

        model.MoveDirection = moveDirection;
        
        return model;
    }

    public BehaviourSoliderController() : base()
    {
    }
}