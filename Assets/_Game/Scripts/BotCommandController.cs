using UniRx;
using UnityEngine;
using Zenject;

public class BotCommandController
{
    private readonly ISelectedElements _selectedElements;
    
    [Inject]
    public BotCommandController(ISelectedElements selectedElements)
    {
        Debug.Log("BotCommandController");
        
        _selectedElements = selectedElements;

        _selectedElements.HitColliders.Subscribe(colliders =>
        {
            foreach (var item in colliders)
            {
                Debug.Log(item);
            }
        });
    }
}