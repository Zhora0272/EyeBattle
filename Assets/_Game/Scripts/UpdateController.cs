using UnityEngine;

public class UpdateController : MonoBehaviour
{
    [SerializeField] private TriggerCheckController _triggerController;

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