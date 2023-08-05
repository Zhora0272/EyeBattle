using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimatedCubesManager : MonoBehaviour
{
    [SerializeField] private Transform _cube;
    [SerializeField] private int _cubesCount;
    [SerializeField] private float _space;

    private List<GameObject> _cubes;
    private float _offset;

    private void OnValidate()
    {
        _cubes ??= new List<GameObject>();

        foreach (var item in _cubes)
        {
            DestroyImmediate(item.gameObject);
        }

        for (int i = 0; i < _cubesCount; i++)
        {
            for (int j = 0; j < _cubesCount; j++)
            {
                var element = Instantiate(_cube, transform);
                element.transform.localPosition = (new Vector3(i * _space,0, j * _space) - new Vector3(_cubesCount / 2, 0, _cubesCount / 2));
                _cubes.Add(element.gameObject);
            }
        }
    }
}
