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

