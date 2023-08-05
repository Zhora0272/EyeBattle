using Lofelt.NiceVibrations;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;
using UniRx;

public class SelectableGrid : MonoBehaviour
{
    [SerializeField] private ParticleSystem _mergeParticle;

    //[SerializeField] private TextMeshProUGUI _weaponLevel;
    [SerializeField] private GameObject _gridObject;
    //[SerializeField] private GameObject _panel;

    private int _stayObjectIndex = -1;
    private Data2048 _data;
    private ISelectableManager _manager;

    public int GetUpgradeIndex()
    {
        return _stayObjectIndex;
    }

    private void OnMouseEnter()
    {
        if (!_gridObject)
        {
            Activate();
        }
    }

    private void OnMouseExit()
    {
        DeActivate();
    }

    public void Activate()
    {
        //_panel.transform.DOLocalMove(new Vector3(0,5,0), 0.5f);
    }

    public void DeActivate()
    {
        //_panel.transform.DOLocalMove(new Vector3(0,0,0), 0.5f);
    }


    public void ShakeObject()
    {
        if (_gridObject)
        {
            _gridObject.transform.DOShakeScale(0.5f, 0.2f).SetEase(Ease.InBounce);
        }
    }
    public bool GetObject(out GameObject item, bool activateState = true)
    {
        if (activateState)
        {
            Activate();
        }

        if (!_gridObject)
        {
            item = null;
            return false;
        }

        item = _gridObject;
        return true;
    }

    public void RemoveObject()
    {
        _gridObject = null;
        _stayObjectIndex = -1;
        //_weaponLevel.text = "";
        DeActivate();
    }

    public bool MoveToObject(GameObject obj, int index = 0)
    {
        if (obj == _gridObject) return false;

        _manager.MergeCallback?.Invoke();

        if (_gridObject == null)
        {
            _stayObjectIndex = index;
            _gridObject = obj;

            if (_gridObject)
            {
                _gridObject.transform.DOMove(transform.position, 0.2f);
                DeActivate();
            }

            //_weaponLevel.text = (_stayObjectIndex + 1).ToString();
            return true;
        }
        else
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);

            Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
            {
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            }).AddTo(this);

            var equalState = _stayObjectIndex == index;
            if (equalState)
            {
                if (_mergeParticle)
                {
                    _mergeParticle.Play();
                }

                obj.transform.DOScale(0, 0.5f).SetEase(Ease.OutBack);
                obj.transform.DOMove(_gridObject.transform.position, 0.5f).OnComplete(() =>
                {
                    Destroy(obj);
                    //_weaponLevel.text = (_stayObjectIndex + 1).ToString();

                    if (_gridObject)
                    {
                        _gridObject.transform.DOMove(transform.position, 0.2f).SetEase(Ease.OutBack);
                        DeActivate();
                    }
                });

                Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
                {
                    _stayObjectIndex++;
                    SetObject(_stayObjectIndex);
                }).AddTo(this);
                
                Destroy(_gridObject);
            }

            return equalState;
        }
    }

    public bool CheckObject()
    {
        return _gridObject != null;
    }

    public void CallToBack()
    {
        DeActivate();
        if (_gridObject)
        {
            if (_gridObject)
            {
                _gridObject.transform.DOMove(transform.position, 0.2f);
            }

            //_weaponLevel.text = (_stayObjectIndex + 1).ToString();
        }
    }

    public void SetManager(ISelectableManager manager, Data2048 data2048)
    {
        _data = data2048;
        _manager = manager;
    }

    public void SetObject(int index = 0, bool particleState = false)
    {
        /*if (particleState && _mergeParticle)
        {
            //_mergeParticle.Play();
        }*/

        _stayObjectIndex = index;

        //_weaponLevel.text = (_stayObjectIndex + 1).ToString();

        _gridObject = Instantiate(_data.GridObject[index]);

        _gridObject.transform.SetParent(transform);

        var size = _gridObject.transform.localScale;

        _gridObject.transform.localScale = Vector3.zero;

        _gridObject.transform.DOScale(size, 0.5f).SetEase(Ease.OutBack);

        _gridObject.transform.localPosition = Vector3.zero;

        _gridObject.transform.localRotation = Quaternion.identity;
    }
}