using Bot.BotController;
using UnityEngine;
using Zenject;

public class BotCommandController
{
    [Inject]
    public BotCommandController(IColliderToBotConvertable colliderToBotConvertable)
    {
        _selectedElements.HitColliders.Subscribe(colliders =>
        {
            if (colliders == null) return;

            foreach (var item in colliders)
            {
                var bot = colliderToBotConvertable.SearchBotAnCollider(item);

                if (bot)
                {
                    var worldPos = _camera.ScreenPointToRay(vector2);
                    Physics.Raycast(worldPos, out var raycastHit);
                }
            }
        });
    }


    internal void SetParameters
    (
        Camera camera
    )
    {
    }
}