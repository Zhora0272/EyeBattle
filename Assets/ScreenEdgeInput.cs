using UnityEngine;

public class ScreenEdgeInput : MonoBehaviour
{
    [SerializeField] private ScreenEdgeInputPart[] _inputParts;

    private void Start()
    {
        _inputParts = GetComponentsInChildren<ScreenEdgeInputPart>();

        foreach (var item in _inputParts)
        {
            item.SetAction(InputAction);
        }
    }

    private void InputAction(Vector2 vector)
    {
        print(vector);
    }
}