using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class BrokenEyePartsController : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Transform[] _transforms;
    [SerializeField] private Rigidbody[] _partsRb;
    [SerializeField] private Renderer[] _partsMaterial;

    [SerializeField] private Mesh[] _partsMeshs;

    [SerializeField] private Vector3[] _partPositions;

    [SerializeField] private MeshCollider[] _collider;


    [ContextMenu("Set Meshes")]
    private void Init()
    {
        for (int i = 0; i < _transforms.Length; i++)
        {
            _transforms[i].GetComponent<MeshFilter>().mesh = _partsMeshs[i];
            _transforms[i].GetComponent<MeshCollider>().sharedMesh = _partsMeshs[i];
            _transforms[i].position = _partPositions[i];
            _collider[i] = _transforms[i].GetComponent<MeshCollider>();
        }
    }

    internal void ReActivate()
    {
        float animTime = 1;
        for (int i = 0; i < _transforms.Length; i++)
        {
            _transforms[i].DOLocalMove(_partPositions[i], animTime);
            _transforms[i].DOLocalRotate(Vector3.zero, animTime);
            _transforms[i].GetComponent<MeshCollider>().enabled = false;
            _collider[i].enabled = false;
            _partsRb[i].isKinematic = true;
        }

        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {   
            gameObject.SetActive(false);
            
        }).AddTo(this);
    }

    public void Activate()
    {
        //transform.SetParent(null);
        gameObject.SetActive(true);

        for (int i = 0; i < _partsRb.Length; i++)
        {
            _collider[i].enabled = true;
            _partsRb[i].isKinematic = false;
            _partsMaterial[i].material = _meshRenderer.material;
        }
    }
}