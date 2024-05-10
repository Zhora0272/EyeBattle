using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UniRx;

public class InputController : MonoManager, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private SmartButton _smartButtons;
    
    public IObservable<Unit> PointerDownStream => _pointerDownSubject;
    public IObservable<Unit> PointerUpStream => _pointerUpSubject;

    public IReactiveProperty<bool> TouchOnScreenState = new ReactiveProperty<bool>(false);

    private Subject<Unit> _pointerDownSubject = new();
    private Subject<Unit> _pointerUpSubject = new();

    private Action<Vector2> _joystickDirection;

    private void OnDisable()
    {
        _joystickDirection?.Invoke(Vector2.zero);
    }

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
        TouchOnScreenState.Value = true;
        _pointerDownSubject.OnNext(default);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TouchOnScreenState.Value = false;
        _pointerUpSubject.OnNext(default);
    }

    public void SmartButtonState(GunType type, ShotType shotType, Action action)
    {
        _smartButtons.IPointerDownHandler.Subscribe(_ =>
        {
            action.Invoke();
        }).AddTo(this);
    }
}

