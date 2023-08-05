using UnityEngine;

[CreateAssetMenu(fileName = "Data2048", menuName = "2048/Data", order = 1)]
public class Data2048 : ScriptableObject
{
    public GameObject[] GridObject;   

    public WorldController WorldPrefab;
}
