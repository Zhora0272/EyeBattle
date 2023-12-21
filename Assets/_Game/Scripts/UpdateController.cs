using UniRx;
using UnityEngine;

public class UpdateController : MonoBehaviour
{
    [SerializeField] private TriggerCheckController _triggerController;

    public IReactiveProperty<UpdateElementController> UpdateElementController => _updateElementController;
    private readonly ReactiveProperty<UpdateElementController> _updateElementController = new();

    private void Awake()
    {
        _triggerController.TriggerEnterRegister(Trigger.UpdateElement, UpdateCheck);
    }

    private void UpdateCheck(Collider other)
    {
        if (other.TryGetComponent<UpdateElementController>(out var result))
        {
        }
    }
}