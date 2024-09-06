using UnityEngine;
using UniRx;

public interface ISelectedElements
{
    public ReactiveProperty<Collider[]> HitColliders { get; }
}

public class SelectionSystemController : ISelectedElements
{
    private readonly ReactiveProperty<Collider[]> _hitColliders = new();

    private Camera _camera;

    public SelectionSystemController()
    {
        _camera = Camera.main;
    }

    public void SelectionVectorsStreamInit(ScreenSelectionVector vectors)
    {
        var startPos = ConvertScreenToWorldPoint(vectors.SelectionStartVector);
        var endPos = ConvertScreenToWorldPoint(vectors.SelectionEndVector);

        Debug.DrawLine(_camera.transform.position, startPos, Color.red);
        Debug.DrawLine(_camera.transform.position, endPos, Color.cyan);

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
        var a = Physics.Raycast(worldPos, out var raycastHit);
        return raycastHit.point;
    }

    public ReactiveProperty<Collider[]> HitColliders => _hitColliders;
}