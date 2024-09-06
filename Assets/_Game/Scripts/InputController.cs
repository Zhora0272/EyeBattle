using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputController : MonoManager
{
    [SerializeField] private InputActionReference spaceAction;
    [SerializeField] private InputAction scrollAction;
    [FormerlySerializedAs("_selectionSystem")] [SerializeField] private SelectionSystemView selectionSystemView;

    public IObservable<Unit> SpacePressStream => _spacePressSubject;
    private Subject<Unit> _spacePressSubject = new();

    private void OnEnable()
    {
        if (scrollAction != null)
        {
            scrollAction.performed += OnScrollPerformed;
            scrollAction.Enable();
        }

        spaceAction.action.performed += OnSpacePressed;
    }

    private void OnDisable()
    {
        if (scrollAction != null)
        {
            scrollAction.performed -= OnScrollPerformed;
            scrollAction.Disable();
        }

        spaceAction.action.performed -= OnSpacePressed;
    }

    private void OnSpacePressed(InputAction.CallbackContext context)
    {
        _spacePressSubject.OnNext(default);
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();
        Debug.Log("Mouse scrolled: " + scrollDelta.y);
    }
}