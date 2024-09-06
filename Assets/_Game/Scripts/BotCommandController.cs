using Bot.BotController;
using UniRx;
using Zenject;

public class BotCommandController
{
    private readonly ISelectedElements _selectedElements;

    [Inject]
    public BotCommandController(ISelectedElements selectedElements, IColliderToBotSearchable colliderToBotSearchable)
    {
        _selectedElements = selectedElements;

        _selectedElements.HitColliders.Subscribe(colliders =>
        {
            if (colliders == null) return;

            foreach (var item in colliders)
            {
                colliderToBotSearchable.SearchBotAnCollider(item);
            }
        });
    }
}
