using Zenject;
using Bot.BotController;
using UnityEngine;
using UniRx;

public class BotCommandController
{
    private readonly IColliderToBotConvertable _colliderToBotConvertable;
    private readonly IScreenPointToWorldPoint _screenPointToWorldPoint;
    private readonly InputController _inputController;
    
    private ReactiveProperty<Collider[]> _hitColliderProperty;

    [Inject]
    public BotCommandController
    (
        IColliderToBotConvertable colliderToBotConvertable,
        IScreenPointToWorldPoint screenPointToWorldPoint,
        InputController inputController
    )
    {
        _inputController = inputController;
        _colliderToBotConvertable = colliderToBotConvertable;
        _screenPointToWorldPoint = screenPointToWorldPoint;
    }


    internal void SetParameters
    (
        ReactiveProperty<Collider[]> hitColliderProperty
    )
    {
        _hitColliderProperty = hitColliderProperty;

        Init();
    }

    private void Init()
    {
        _inputController.MouseButtonProperty.Subscribe(mouseButton =>
        {
            if (mouseButton != null)
            {
                if (mouseButton.ButtonType == MouseButton.leftButton)
                {
                    if (_hitColliderProperty.Value != null)
                    {
                        foreach (var item in _hitColliderProperty.Value)
                        {
                            var bot = _colliderToBotConvertable.SearchBotWithCollider(item);
                            if (bot != null)
                            {
                                var position = _screenPointToWorldPoint.ScreenPointInWorld(mouseButton.Position);
                                bot.SetTargetPosition(position);
                            }
                        }
                    }
                }
                else if (mouseButton.ButtonType == MouseButton.rightButton)
                {
                    _hitColliderProperty = null;
                }
            }
        });
    }
}