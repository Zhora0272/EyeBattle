using UnityEngine;
using UniRx;

public class ScreenInputEventController
{
    private Camera _camera;

    public ScreenInputEventController()
    {
        _camera = Camera.main;
    }

    public ReactiveProperty<ScreenSelectRangePoint> SelectionVectorProperty;

}