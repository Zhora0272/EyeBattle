using System.Collections.Generic;
using UnityEngine;

public class EyeSpawnManager : MonoManager
{
    [SerializeField] private GameObject _botPrrefab;

    public List<Transform> _spawnEyes { private set; get; }

    protected override void Awake()
    {
        base.Awake();

        _spawnEyes = new List<Transform>();
    }

    private void Start()
    {
        
    }
}
