using Lofelt.NiceVibrations;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;
using UniRx;

public class SelectableGrid : MonoBehaviour
{
    //[SerializeField] private ParticleSystem _mergeParticle;
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
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        if (!_gridObject)
        {
            Activate();
        }
    }

    private void Awake()
    {
        //_weaponLevel.text = "";
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
        
        if (_gridObject == null)
        {
            _stayObjectIndex = index;
            _gridObject = obj;

            if (_gridObject)
            {
                _gridObject.transform.DOMove(transform.position + transform.up * 0.2f, 0.2f);
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
                _manager.MergeCallback.Invoke();
               /* if (_mergeParticle)
                {
                    _mergeParticle.Play();
                }*/

                Destroy(obj);
                Destroy(_gridObject);
                
                _stayObjectIndex++;
                //_weaponLevel.text = (_stayObjectIndex + 1).ToString();

                if (_gridObject)
                {
                    _gridObject.transform.DOMove(transform.position + transform.up * 0.2f, 0.2f);
                    DeActivate();
                }

                SetObject(_stayObjectIndex);
            }
            
            return equalState;
        }
    }

    public void CallToBack()
    {
        DeActivate();
        if (_gridObject)
        {
            if (_gridObject)
            {
                _gridObject.transform.DOMove(transform.position + transform.up * 0.2f, 0.2f);
            }

            //_weaponLevel.text = (_stayObjectIndex + 1).ToString();
        }
    }

    public void SetManager(ISelectableManager manager)
    {
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
        
        _gridObject.transform.SetParent(transform.parent.transform.parent);
        
        _gridObject.transform.localScale *= 0.07f; 
        _gridObject.transform.localEulerAngles = new Vector3(-0.434f,-41.959f,10);
    }
}
