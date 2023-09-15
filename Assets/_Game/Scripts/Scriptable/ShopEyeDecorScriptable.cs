using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeDecorData", menuName = "Data/Shop/EyeDecorData")]
public class ShopEyeDecorScriptable : ScriptableObject
{
    [field:SerializeField] public GameObject[] Decors { get; private set; }
}