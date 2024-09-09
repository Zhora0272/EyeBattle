using UnityEngine.InputSystem;
using UnityEngine;
using UniRx;

public enum MouseButton
{
    leftButton,
    rightButton,
    middleButton
}

public class MouseInputData
{
    public MouseButton ButtonType;
    public Vector2 Position;
}

public class InputController : MonoManager, IScreenSelectRangePoint, IScreenClickPoint
{
    [SerializeField] private InputActionReference spaceAction;
    [SerializeField] private InputActionReference mouseClickAction;

    [SerializeField] private InputAction scrollAction;
    [SerializeField] private ScreenInputEventView screenInputEventView;
    [SerializeField] private ScreenEdgeInput _screenEdgeInput;

    public IReactiveProperty<Unit> SpacePressProperty => _spacePressProperty;
    private readonly ReactiveProperty<Unit> _spacePressProperty = new();

    #region MouseProperty

    public IReactiveProperty<float> MouseScrollProperty => _mouseScrollProperty;
    private readonly ReactiveProperty<float> _mouseScrollProperty = new();
    
    public IReactiveProperty<MouseInputData> MouseButtonProperty => _mouseButtonProperty;
    private readonly ReactiveProperty<MouseInputData> _mouseButtonProperty = new();

    #endregion

    #region ScreenProperty

    public IReactiveProperty<ScreenSelectRangePoint> SelectRangePointProperty =>
        screenInputEventView.SelectRangePoint;
    
    public IReactiveProperty<Vector2> ScreenClickPointProperty =>
        screenInputEventView.ScreenClickProperty;

    public IReactiveProperty<Vector2> ScreenEdgeInputProperty =>
        _screenEdgeInput.OutputValueProperty;

    #endregion

    private void OnEnable()
    {
        if (mouseClickAction)
        {
            mouseClickAction.action.performed += OnMouseClick;
            mouseClickAction.action.Enable();
        }

        if (scrollAction != null)
        {
            scrollAction.performed += OnScrollPerformed;
            scrollAction.Enable();
        }

        if (spaceAction)
        {
            spaceAction.action.performed += OnSpacePressed;
        }
    }

    private void OnDisable()
    {
        if (mouseClickAction)
        {
            mouseClickAction.action.performed -= OnMouseClick;
            mouseClickAction.action.Disable();
        }

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

        var inputData = new MouseInputData();
        
        string buttonPrefix = "/Mouse/";
        
        if (control.path == buttonPrefix + MouseButton.leftButton)
        {
            inputData.ButtonType = MouseButton.leftButton;
        }
        else if (control.path == buttonPrefix + MouseButton.rightButton)
        {
            inputData.ButtonType = MouseButton.rightButton;
        }
        else if (control.path == buttonPrefix + MouseButton.middleButton)
        {
            inputData.ButtonType = MouseButton.middleButton;
        }

        inputData.Position = Input.mousePosition;
        _mouseButtonProperty.SetValueAndForceNotify(inputData);
    }
}