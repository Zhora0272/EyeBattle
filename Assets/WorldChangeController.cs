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
        var index = PlayerPrefs.GetInt(ShopParameters.ShopSelectedTheme.ToString());
        ChangeTheme(index);
    }

    private void Start()
    {
        MainManager.GetManager<UIManager>().SubscribeToPageDeactivate(UIPageType.Shop,
       () =>
       {
           var index = PlayerPrefs.GetInt(ShopParameters.ShopSelectedTheme.ToString());
           ChangeTheme(index);
       });
    }

    public void ChangeTheme(int index)
    {
        _worldInverseState = !_worldInverseState;

        transform.DORotate(_worldInverseState ? Vector3.zero : new Vector3(180, 0, 0), 0.7f);

       Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
       {
           SetWorld(_datas[index]);

       }).AddTo(this);
    }


    public Transform GetWorldSpawntransform()
    {
        return _worldInverseState ?
            _worldUpTransform :
            _worldButtomTransform;
    }

    public void SetWorld(Data2048 data)
    {
        if (_currentWorld)
        {
            _currentWorld.transform.DOScale(0, 0.45f);
            Observable.Timer(TimeSpan.FromSeconds(0.45f)).Subscribe(_ =>
            {
                Destroy(_currentWorld.gameObject);

            }).AddTo(this);
        }

        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            _currentWorld = Instantiate(data.WorldPrefab, GetWorldSpawntransform());
            
            _currentWorld.transform.localPosition = new Vector3(-2.5f, 0, 0);
            _currentWorld.transform.localRotation = Quaternion.identity;

        }).AddTo(this);
    }

}
