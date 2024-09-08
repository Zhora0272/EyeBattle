using UniRx;
using UnityEngine;

public class ScreenToWorldCastController
{
    private readonly ReactiveProperty<Collider[]> _hitColliders = new();

    private IScreenSelectRangePoint _screenSelectRangePoint;
    
    private Camera _camera;

    public ScreenToWorldCastController()
    {
        _hitColliders.Subscribe(colliders => { });
    }

    public void Init
    (
        Camera camera,
        IScreenSelectRangePoint screenSelectRangePoint
    )
    {
        _screenSelectRangePoint = screenSelectRangePoint;
        _camera = camera;
    }

    public void SetHitColliders(Collider[] colliders)
    {
        _hitColliders.SetValueAndForceNotify(colliders);
    }

    public void SetSelectedScreenPoint(ScreenSelectRangePoint rangePoints)
    {
        var startPos = ConvertScreenToWorldPoint(rangePoints.SelectionStartVector);
        var endPos = ConvertScreenToWorldPoint(rangePoints.SelectionEndVector);

        Vector3 center = (startPos + endPos) / 2;
        Vector3 halfExtents =
            new Vector3(Mathf.Abs(endPos.x - startPos.x) / 2,
                Mathf.Abs(endPos.y - startPos.y) / 2,
                1f);

        _hitColliders.Value = Physics.OverlapBox(center, halfExtents, Quaternion.identity);
    }

    private Vector3 ConvertScreenToWorldPoint(Vector2 vector2)
    {
        if (_camera == null)
        {
            Debug.LogError("Camera is not assigned!");
            return Vector3.zero;
        }

        var worldPos = _camera.ScreenPointToRay(vector2);
        Physics.Raycast(worldPos, out var raycastHit);
        return raycastHit.point;
    }
}