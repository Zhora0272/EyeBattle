using UnityEngine;
using Zenject;

public class BotCommandView : MonoBehaviour
{
    private BotCommandController _botCommandController;
    private ISelectedElements _selectedElements;

    [Inject]
    private void Constructor
    (
        ISelectedElements selectedElements,
        BotCommandController botCommandController
    )
    {
        _selectedElements = selectedElements;
        _botCommandController = botCommandController;

        _botCommandController.SetParameters(_selectedElements.HitColliders);
    }
}