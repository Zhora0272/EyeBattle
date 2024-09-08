using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


public class ScreenInputEventView : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
{
    [Inject] private ScreenInputEventController _screenInputEventController;
    
    [SerializeField] private RectTransform _selectionRectransform;

    public IReactiveProperty<ScreenSelectRangePoint> SelectRangePoint => _selectRangePoint;
    public ReactiveProperty<ScreenSelectRangePoint> _selectRangePoint;

    private Vector2 _pointerDownVector;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownVector = eventData.position;
        _selectionRectransform.anchoredPosition = _pointerDownVector;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _selectRangePoint.Value = new ScreenSelectRangePoint(_pointerDownVector, eventData.position);
        
        _pointerDownVector = Vector2.zero;
        _selectionRectransform.sizeDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var vector = new Vector2(eventData.position.x, eventData.position.y);
        var delta = new Vector2(vector.x - _pointerDownVector.x, vector.y - _pointerDownVector.y);

        _selectionRectransform.eulerAngles = new Vector3(delta.y <= 0 ? 180 : 0, delta.x <= 0 ? 180 : 0);
        _selectionRectransform.sizeDelta = new Vector2(Math.Abs(delta.x), Math.Abs(delta.y));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}