using UnityEngine;
using UniRx;

public interface IScreenPointToWorldPoint
{
    public Vector3 ScreenPointInWorld(Vector2 vector2);
}

public class ScreenToWorldCastController : IScreenPointToWorldPoint
{
    private ReactiveProperty<Collider[]> _colliderProperty;
    
    private IScreenSelectRangePoint _screenSelectRangePoint;
    
    private Camera _camera;
    
    public void Init
    (
        Camera camera,
        IScreenSelectRangePoint screenSelectRangePoint,
        ReactiveProperty<Collider[]> colliderProperty
    )
    {
        _screenSelectRangePoint = screenSelectRangePoint;
        _colliderProperty = colliderProperty;
        _camera = camera;

        _screenSelectRangePoint.SelectRangePointProperty.Subscribe(SetSelectedScreenPoint);
    }

    private void SetSelectedScreenPoint(ScreenSelectRangePoint rangePoints)
    {
        var startPos = ConvertScreenToWorldPoint(rangePoints.SelectionStartVector);
        var endPos = ConvertScreenToWorldPoint(rangePoints.SelectionEndVector);

        Vector3 center = (startPos + endPos) / 2;
        Vector3 halfExtents =
            new Vector3(Mathf.Abs(endPos.x - startPos.x) / 2,
                Mathf.Abs(endPos.y - startPos.y) / 2,
                1f);
        
        _colliderProperty.Value = Physics.OverlapBox(center, halfExtents, Quaternion.identity);
    }
    
    public Vector3 ScreenPointInWorld(Vector2 vector2)
    {
        var ray = _camera.ScreenPointToRay(vector2);
        Physics.Raycast(ray, out var hit);

        return hit.point;
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