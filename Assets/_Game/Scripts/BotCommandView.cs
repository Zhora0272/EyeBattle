using UnityEngine;
using Zenject;

public class BotCommandView : MonoBehaviour
{
    [SerializeField] private InputController _inputController;
    private BotCommandController _botCommandController;
    
    private ISelectedElements _selectedElements;

    [Inject]
    private void Constructor
    (
        ISelectedElements selectedElements,
        BotCommandController botCommandController
    )
    {
        _botCommandController = botCommandController;
        _selectedElements = selectedElements;

        _botCommandController.SetParameters(_selectedElements.HitColliders, _inputController);
    }
}