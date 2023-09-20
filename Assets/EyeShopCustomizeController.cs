using UnityEngine;

public class EyeShopCustomizeController : MonoBehaviour
{
    [Header("PlayerEyeParameters")] [SerializeField]
    private EyeCustomizeController _eyeCustomizeController;

    [Header("ShopEyeParameters")] [SerializeField]
    private MeshRenderer _meshRenderer;

    private Material _eyeMaterial;
    private GameObject _eyeDecor;

    private void OnEnable()
    {
        _eyeMaterial = _eyeCustomizeController.GetMaterial();
        _eyeDecor = _eyeCustomizeController.GetDecor();

        Init();
    }

    private void Init()
    {
        if (_eyeDecor)
        {
            var item = Instantiate(_eyeDecor, transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = _eyeDecor.transform.localRotation;
        }

        _meshRenderer.material = _eyeMaterial;
    }

    private void Awake()
    {
    }
}