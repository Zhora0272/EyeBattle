using DG.Tweening;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public abstract class EyeBaseController : MonoBehaviour 
{
    [SerializeField] protected BrokenEyePartsController _brokenEyePartsController;
    [SerializeField] protected BrokenEyeCollection _brokenEyeCollector;

    [SerializeField] protected Rigidbody Rb;
    [SerializeField] protected int Hp;
    [Space]
    [SerializeField] protected Material _material;
    [SerializeField] protected GameObject _meshRenderer;
    [SerializeField] protected SphereCollider _sphereCollider;
    [Space]
    [SerializeField] protected Image _loadbar;
    [Space]
    [SerializeField] protected float Speed;
    
    [field:SerializeField] public ReactiveProperty<float> Size { protected set; get; }
    [field:SerializeField] public bool IsDeath { protected set; get; }

    protected Vector2 _moveDirection;

    private void Start()
    {
        _brokenEyeCollector.BrokenPartsCollectionStream.Subscribe(value =>
        {
            Size.Value += value;

        }).AddTo(this);

        Size.Subscribe(value =>
        {
            _loadbar.DOFillAmount(((int)(value / 100) + 1) - ((float)value / 100), 1);

            if (value % 100 == 0)
            {
                transform.DOScale((value / 300) + 1, 1);

                _loadbar.DOKill();
                _loadbar.DOFillAmount(0, 1);
            }

        }).AddTo(this);
    }

    protected virtual void Update()
    {
        Rb.velocity = Vector3.Lerp(Rb.velocity, Vector3.zero, Time.deltaTime);
        Rb.angularVelocity = Vector3.Lerp(Rb.angularVelocity, Vector3.zero, Time.deltaTime);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Eye"))
        {
            if (collision.gameObject.TryGetComponent<EyeBaseController>(out var result))
            {
                result.Attack(Rb.mass * Rb.velocity.magnitude, transform.position);
            }
        }
    }

    protected virtual void Attack(float force, Vector3 attackPosition)
    {
        if(Hp < force)
        {
            IsDeath = true;

            _brokenEyePartsController.Activate(_material, attackPosition);

            _sphereCollider.enabled = false;
            _meshRenderer.SetActive(false);
            Rb.isKinematic = true;
        }
    }

}
