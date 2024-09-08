using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenEdgeInputPart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector2 _inputVector;

    private IDisposable _everyUpdate;

    private Action<Vector2> _action;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _everyUpdate = Observable.EveryUpdate().Subscribe(_ =>
        {
            _action.Invoke(_inputVector);
        }).AddTo(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _action.Invoke(Vector2.zero);
        _everyUpdate.Dispose();
    }

    public void SetAction(Action<Vector2> action) => _action = action;

}