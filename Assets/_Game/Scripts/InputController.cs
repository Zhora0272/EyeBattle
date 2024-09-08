using UnityEngine.InputSystem;
using UnityEngine;
using UniRx;

public enum MouseButton
{
    leftButton,
    rightButton,
    middleButton
}

public interface IScreenSelectRangePoint
{
    public IReactiveProperty<ScreenSelectRangePoint> SelectRangePointProperty { get; }
}
public class InputController : MonoManager, IScreenSelectRangePoint
{
    [SerializeField] private InputActionReference spaceAction;
    [SerializeField] private InputActionReference mouseClickAction;
    
    [SerializeField] private InputAction scrollAction;
    [SerializeField] private ScreenInputEventView screenInputEventView;
    [SerializeField] private ScreenEdgeInput _screenEdgeInput;

    public IReactiveProperty<Unit> SpacePressProperty => _spacePressProperty;
    private readonly ReactiveProperty<Unit> _spacePressProperty = new();

    public IReactiveProperty<float> MouseScrollProperty => _mouseScrollProperty;
    private readonly ReactiveProperty<float> _mouseScrollProperty = new();
    
    public IReactiveProperty<MouseButton> MouseButtonProperty => _mouseButtonProperty;
    private readonly ReactiveProperty<MouseButton> _mouseButtonProperty = new();
    
    public IReactiveProperty<ScreenSelectRangePoint> SelectRangePointProperty => screenInputEventView.SelectRangePoint;

    public IReactiveProperty<Vector2> ScreenEdgeInputProperty => _screenEdgeInput.OutputValueProperty;

    private void OnEnable()
    {
        if (scrollAction != null)
        {
            scrollAction.performed += OnScrollPerformed;
            scrollAction.Enable();
        }

        if (spaceAction)
        {
            spaceAction.action.performed += OnSpacePressed;
        }
        
        mouseClickAction.action.performed += OnMouseClick;
        mouseClickAction.action.Enable();
    }

    private void OnDisable()
    {
        mouseClickAction.action.performed -= OnMouseClick;
        mouseClickAction.action.Disable();
        
        if (scrollAction != null)
        {
            scrollAction.performed -= OnScrollPerformed;
            scrollAction.Disable();
        }

        if (spaceAction)
        {
            spaceAction.action.performed -= OnSpacePressed;
        }
    }

    private void OnSpacePressed(InputAction.CallbackContext context)
    {
        _spacePressProperty.SetValueAndForceNotify(default);
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();
        _mouseScrollProperty.SetValueAndForceNotify(scrollDelta.y);
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        var control = context.control;

        string buttonPrefix = "<Mouse>/";

        if (control.path == buttonPrefix + MouseButton.leftButton)
        {
            _mouseButtonProperty.SetValueAndForceNotify(MouseButton.leftButton);
        }
        else if (control.path == buttonPrefix + MouseButton.rightButton)
        {
            _mouseButtonProperty.SetValueAndForceNotify(MouseButton.rightButton);
        }
        else if (control.path == buttonPrefix + MouseButton.middleButton)
        {
            _mouseButtonProperty.SetValueAndForceNotify(MouseButton.middleButton);
        }
    }
}