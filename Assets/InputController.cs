using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    [SerializeField] private FloatingJoystick _joystick;

    private Action<Vector2> _joystickDirection;

    private void Update()
    {
        if(_joystickDirection != null)
        {
            _joystickDirection.Invoke(_joystick.Direction);
        }
    }

    public void RegisterJoysticData(Action<Vector2> joystickDirection) 
    {
        _joystickDirection += joystickDirection;
    }
}
