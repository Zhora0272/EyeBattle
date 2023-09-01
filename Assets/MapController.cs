using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private List<Transform> _elements;

    private void Start()
    {
        _elements = MainManager.GetManager<EyeSpawnManager>()._spawnEyes;
    }

    private void Update()
    {
        if(_elements != null)
        {

        }
    }
}
