using DG.Tweening;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [field: SerializeField] private CollectableType _type;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshCollider _collider;

    public void Collect()
    {
        _rb.isKinematic = true;
        _collider.enabled = false;

        transform.DOScale(0, 2).onComplete = () =>
        {
            gameObject.SetActive(false);
        };
    }
}

public enum CollectableType
{
    BrokenEye,   
}
