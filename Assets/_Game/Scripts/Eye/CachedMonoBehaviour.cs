using UnityEngine;

public class CachedMonoBehaviour : MonoBehaviour
{
    private Transform _thisTransform;
    public Transform ThisTransform {
        get {
            if (!_thisTransform) {
                _thisTransform = transform;
            }
            return _thisTransform;
        }
    }
  
    private Vector3 _position;
    public Vector3 Position {
        get => ThisTransform.position;
        set => ThisTransform.position = value;
    }
  
    private Quaternion _rotation;
    public Quaternion Rotation {
        get => ThisTransform.rotation;
        set => ThisTransform.rotation = value;
    }   
}