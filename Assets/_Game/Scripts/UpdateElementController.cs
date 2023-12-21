using UnityEngine;

public class UpdateElementController : MonoBehaviour
{
    [field: SerializeField] public UpdateElement UpdateElementType { get; private set; }
}

public class UpdateElementBehaviour : CachedMonoBehaviour
{
}

public enum UpdateElement
{
    Speed,
}

public abstract class UpdateElementModel : EyeModelBase
{
    public UpdateElement UpdateType;
    public float UpdateTime; // if time is (< 0) update is always
}