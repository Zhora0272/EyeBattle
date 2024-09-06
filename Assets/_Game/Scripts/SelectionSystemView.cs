using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


public class SelectionSystemView : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform _selectionRectransform;

    [Inject] private SelectionSystemController _selectionSystemController;

    private Vector2 _pointerDownVector;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownVector = eventData.position;
        _selectionRectransform.anchoredPosition = _pointerDownVector;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _selectionSystemController.SelectionVectorsStreamInit(
            new ScreenSelectionVector(_pointerDownVector, eventData.position));
        
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
}