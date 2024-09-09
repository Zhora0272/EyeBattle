using Bot.BotController;
using UniRx;
using UnityEngine;
using Zenject;

public class BotCommandController
{
    private IColliderToBotConvertable _colliderToBotConvertable;
    private IScreenPointToWorldPoint _screenPointToWorldPoint;

    private InputController _inputController;
    private ReactiveProperty<Collider[]> _hitColliderProperty;

    [Inject]
    public BotCommandController
    (
        IColliderToBotConvertable colliderToBotConvertable,
        IScreenPointToWorldPoint screenPointToWorldPoint
    )
    {
        _colliderToBotConvertable = colliderToBotConvertable;
        _screenPointToWorldPoint = screenPointToWorldPoint;
    }


    internal void SetParameters
    (
        ReactiveProperty<Collider[]> hitColliderProperty,
        InputController inputController
    )
    {
        _inputController = inputController;
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
                            var bot = _colliderToBotConvertable.SearchBotAnCollider(item);
                            if (bot != null)
                            {
                                var position = _screenPointToWorldPoint.ScreenPointInWorld(mouseButton.Position);
                                bot.SetTargetPosition(position);
                            }
                        }
                    }
                }
            }
        });
    }
}