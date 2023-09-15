using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeSizeData", menuName = "Data/Shop/EyeSizeData")]
public class ShopEyeSizeScriptable : MonoBehaviour
{
    [field:SerializeField] public int[] EyeSize { get; private set; }
}
