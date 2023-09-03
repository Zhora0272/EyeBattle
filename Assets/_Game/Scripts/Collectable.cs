using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [field: SerializeField] private CollectableType _type;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshCollider _collider;
    [SerializeField] private float _value;

    public bool CollectState { private set; get; }
    public float BrokenTime{ private set; get; }

    private void OnEnable()
    {
        BrokenTime = Time.time;
    }

    public float Collect(Vector3 collectPosition)
    {
        CollectState = true;

        _collider.enabled = false;
        _rb.isKinematic = true;
        transform.DOScale(0, 0.5f).onComplete = () =>
        {
            gameObject.SetActive(false);
        };

        return _value;
    }
}

public enum CollectableType
{
    BrokenEye,   
}
