using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeSpawnManager : MonoManager
{
    [SerializeField] private EyeBaseController _botPrrefab;
    [SerializeField] private EyeBaseController _playerTransform;

    public List<EyeBaseController> _spawnEyes { private set; get; }

    protected override void Awake()
    {
        base.Awake();

        _spawnEyes = new List<EyeBaseController>();
    }

    //need pooling system
    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(_ => 
        {
            var position = _playerTransform.transform.position;

            var randomPosition = new Vector3(
                position.x + Random.Range(-14,14),
                0,
                position.z + Random.Range(-14, 14));

            var item = Instantiate(_botPrrefab, randomPosition, Quaternion.identity);

            _spawnEyes.Add(item);

        }).AddTo(this);
    }
}
