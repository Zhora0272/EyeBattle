using UniRx;
using UnityEngine;

public class UpdateController : MonoBehaviour
{
    [SerializeField] private TriggerCheckController _triggerController;

    public IReactiveProperty<UpdateElementModel> UpdateElementController => _updateElementController;
    private readonly ReactiveProperty<UpdateElementModel> _updateElementController = new();

    private void Awake()
    {
        _triggerController.TriggerEnterRegister(Trigger.UpdateElement, UpdateCheck);
    }

    // this function is incorrect but timed solution //
    private void UpdateCheck(Collider other)
    {
        if (other.TryGetComponent<UpdateElementController>(out var result))
        {
            _updateElementController.Value = result.UpdateElementModel;
            result.UpdateElementModel.IsApplied.Value = true;
        }
    }
}