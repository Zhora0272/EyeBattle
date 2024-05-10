using System;
using UniRx;
using UnityEngine;

public class EyeShopCustomizeController : MonoBehaviour
{
    [Header("PlayerEyeParameters")] [SerializeField]
    private EyeCustomizeController _eyeCustomizeController;

    [Header("ShopEyeParameters")] [SerializeField]
    private MeshRenderer _meshRenderer;

    private Material _eyeMaterial;
    private GameObject _eyeDecor;

    private void Awake()
    {
        _eyeMaterial = _eyeCustomizeController.GetMaterial();
        _eyeCustomizeController.ReactiveDecorGameObject.Subscribe(decor =>
        {
            if (decor)
            {
                var item = Instantiate(_eyeDecor, transform);
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = _eyeDecor.transform.localRotation;
            }
            
        }).AddTo(this);

        Init(); 
    }

    private void Init()
    {
        _meshRenderer.material = _eyeMaterial;
    }
}