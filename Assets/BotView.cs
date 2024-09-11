using UnityEngine;
using UnityEngine.EventSystems;

public class BotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UiInfoView _uiInfoView;

    private BotParameters _botParameters;

    private void Start()
    {
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //_uiInfoView.Activate();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        _uiInfoView.Deactivate();
    }
}