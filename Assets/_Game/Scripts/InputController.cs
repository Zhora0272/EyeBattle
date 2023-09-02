using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UniRx;

public class InputController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private FloatingJoystick _joystick;

    public IObservable<Unit> PointerDownStream => _pointerDownSubject;
    public IObservable<Unit> PointerUpStream => _pointerUpSubject;

    private Subject<Unit> _pointerDownSubject = new();
    private Subject<Unit> _pointerUpSubject = new();

    private Action<Vector2> _joystickDirection;

    public void RegisterJoysticData(Action<Vector2> joystickDirection) 
    {
        _joystickDirection += joystickDirection;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _joystickDirection?.Invoke(_joystick.Direction);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownSubject.OnNext(default);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerUpSubject.OnNext(default);
    }
}
