using System;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class SmartButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    internal IObservable<Unit> IPointerDownHandler => _pointerDownHandlerDisposable;
    internal IObservable<Unit> IPointerUpHandler => _pointerUpHandlerDisposable;
    internal IObservable<Unit> IDragHandler => _pointerDragHandlerDisposable;
    internal IObservable<Unit> IDoubleClick => _doubleClickDisposable;

    private readonly Subject<Unit> _pointerDownHandlerDisposable = new();
    private readonly Subject<Unit> _pointerUpHandlerDisposable = new();
    private readonly Subject<Unit> _pointerDragHandlerDisposable = new();
    private readonly Subject<Unit> _doubleClickDisposable = new();

    private float _lastClickTime;

    public void ForgotAllEvents()
    {
        _doubleClickDisposable?.Dispose();
        _pointerDragHandlerDisposable?.Dispose();
        _pointerUpHandlerDisposable?.Dispose();
        _pointerDownHandlerDisposable?.Dispose();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Time.time - _lastClickTime < 0.25f)
        {
            _doubleClickDisposable.OnNext(default);
        }
        _lastClickTime = Time.time;
        
        _pointerDownHandlerDisposable.OnNext(default);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerUpHandlerDisposable.OnNext(default);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _pointerDragHandlerDisposable.OnNext(default);
    }
}
