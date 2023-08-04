using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class SelectablePlane : MonoBehaviour, ISelectableManager
{
    private SelectableGrid[] _selectableGrid;
    [field:SerializeField] public Data2048 Data { get; private set; }

    public Action MergeCallback { get; private set; }

    private enum PlayerPrefsEnum
    {
        WeaponMergeBuyCount,
        WeaponMergeSaveIndex,
    }

    private void Awake()
    {
        _selectableGrid = GetComponentsInChildren<SelectableGrid>();

        for (int i = 0; i < _selectableGrid.Length; i++)
        {
            //var index = PlayerPrefs.GetInt(PlayerPrefsEnum.WeaponMergeSaveIndex.ToString() + i, -1);

            _selectableGrid[i].SetManager(this, Data);

            /*if (index != -1)
            {
                _selectableGrid[i].SetObject(index);
            }*/
        }

        var mergeCount = PlayerPrefs.GetInt(PlayerPrefsEnum.WeaponMergeBuyCount.ToString(), 0);

        _selectableGrid[0].SetObject();
        _selectableGrid[Random.Range(1, _selectableGrid.Length - 1)].SetObject();

        MergeCallback = () => 
        {
            var freeGrides = from item in _selectableGrid where (!item.CheckObject()) select (item);
            var count = freeGrides.Count();
            var randomIndex = Random.Range(0, count - 1);

            freeGrides.ToArray()[randomIndex].SetObject();
        };

    }

    private void Start()
    { 
        var mergeCount = PlayerPrefs.GetInt(PlayerPrefsEnum.WeaponMergeBuyCount.ToString(), 0);
        
        if (mergeCount == 0)
        {
            bool state = false;
            
            for (int i = 0; i < _selectableGrid.Length; i++)
            {
                if (_selectableGrid[i].GetObject(out var rs, false))
                {
                    state = true;
                }
            }
            if (!state)
            {
                _selectableGrid[0].SetObject();
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _selectableGrid.Length; i++)
        {
            PlayerPrefs.SetInt(PlayerPrefsEnum.WeaponMergeSaveIndex.ToString() + i,
                _selectableGrid[i].GetUpgradeIndex());
        }
    }
}

public interface ISelectableManager
{
    public Data2048 Data { get; }
    public Action MergeCallback { get; }  
}