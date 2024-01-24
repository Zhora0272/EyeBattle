using System;
using UnityEngine;

public class CachedMonoBehaviour : MonoBehaviour, ITransform
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
    
    public Vector3 LocalPosition {
        get => ThisTransform.localPosition;
        set => ThisTransform.localPosition = value;
    }
  
    private Quaternion _rotation;
    public Quaternion Rotation {
        get => ThisTransform.rotation;
        set => ThisTransform.rotation = value;
    }

    public Vector3 IPosition => Position;
    public Vector3 ILocalPosition => LocalPosition;
    public Quaternion IRotation => Rotation;
    public String IGameObjectName => gameObject.name;
}

public interface ITransform
{
    public Vector3 IPosition { get; }
    public Vector3 ILocalPosition { get; }
    public Quaternion IRotation { get; }
    public string IGameObjectName { get; }
}