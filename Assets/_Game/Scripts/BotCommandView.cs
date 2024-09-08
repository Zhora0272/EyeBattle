using UniRx;
using UnityEngine;
using Zenject;

public class BotCommandView : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    
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
        
        _selectedElements.HitColliders.Subscribe(collider =>
        {
            
        }).AddTo(this);
        
        _botCommandController.SetParameters(_camera);
    }
}