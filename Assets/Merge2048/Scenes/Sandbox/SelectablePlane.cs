using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class SelectablePlane : MonoBehaviour, ISelectableManager
{
    private SelectableGrid[] _selectableGrid;
    [SerializeField] public Data2048 Data { get; private set; }

    public Action MergeCallback { get; private set; }

    private enum PlayerPrefsEnum
    {
        WeaponMergeBuyCount,
        WeaponMergeSaveIndex,
    }

    private void Awake()
    {
        var mergeCount = PlayerPrefs.GetInt(PlayerPrefsEnum.WeaponMergeBuyCount.ToString(), 0);

        MergeCallback = () => { };

        _selectableGrid = GetComponentsInChildren<SelectableGrid>();
    }

    private void Start()
    {
        for (int i = 0; i < _selectableGrid.Length; i++)
        {
            var index = PlayerPrefs.GetInt(PlayerPrefsEnum.WeaponMergeSaveIndex.ToString() + i, -1);

            _selectableGrid[i].SetManager(this);

            if (index != -1)
            {
                _selectableGrid[i].SetObject(index);
            }
        }
        
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