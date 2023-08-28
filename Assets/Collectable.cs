using UnityEngine;

public class Collectable : MonoBehaviour
{
    [field: SerializeField] private CollectableType _type;
}

public enum CollectableType
{
    BrokenEye,   
}
