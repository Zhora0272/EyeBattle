using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class WorldChangeController : MonoBehaviour
{
    [SerializeField] private Data2048[] _datas;

    [SerializeField] private Transform _worldUpTransform;
    [SerializeField] private Transform _worldButtomTransform;

    private WorldController _currentWorld;

    private bool _worldInverseState;

    private void Awake()
    {
        ChangeTheme(0);
    }

    public void ChangeTheme(int index)
    {
        _worldInverseState = !_worldInverseState;

        transform.DORotate(_worldInverseState ? Vector3.zero : new Vector3(180, 0, 0), 0.7f);

        SetWorld(_datas[index]);
    }


    public Transform GetWorldSpawntransform()
    {
        return _worldInverseState ?
            _worldUpTransform :
            _worldButtomTransform;
    }

    public void SetWorld(Data2048 data)
    {
        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            _currentWorld = Instantiate(data.WorldPrefab, GetWorldSpawntransform());
            
            _currentWorld.transform.localPosition = new Vector3(-2.5f, 0, 0);
            _currentWorld.transform.localRotation = Quaternion.identity;

        }).AddTo(this);
    }

}
