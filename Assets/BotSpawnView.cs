using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BotSpawnView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private BotParameters _botParameters;
    [SerializeField] private MovableBattleParticipantBaseController _botPrrefab;

    [Inject] private BotSpawnController _botSpawnController;

    public void TryUpdateParameters()
    {
        if (_botSpawnController.TryUpdateParameters(out var result))
        {
            
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
}