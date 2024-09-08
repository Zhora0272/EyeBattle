using UniRx;
using UnityEngine;

public class ScreenEdgeInput : MonoBehaviour
{
    [SerializeField] private ScreenEdgeInputPart[] _inputParts;

    public IReactiveProperty<Vector2> OutputValueProperty => _outputValueProperty;
    private readonly ReactiveProperty<Vector2> _outputValueProperty = new();

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
        _outputValueProperty.SetValueAndForceNotify(vector);
    }
}