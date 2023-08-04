using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SelectablePlane : MonoBehaviour, ISelectableManager
{
    private SelectableGrid[] _selectableGrid;

    [SerializeField] private TextMeshProUGUI _upgradePrice;
    [SerializeField] private GameObject[] _upgradeDisableObjects;
    
    [SerializeField] private Button _buyElement;
    [SerializeField] public Button _fight;

    [SerializeField] private int[] _weaponBuy;
    

    private void OnEnable()
    {
        foreach (var item in _upgradeDisableObjects)
        {
            item.SetActive(false);
        }
    }

    private enum PlayerPrefsEnum
    {
        WeaponMergeBuyCount,
        WeaponMergeSaveIndex,
    }

    private void Awake()
    {
        var mergeCount = PlayerPrefs.GetInt(PlayerPrefsEnum.WeaponMergeBuyCount.ToString(), 0);
        
        _fight.onClick.AddListener(() =>
        {
            foreach (var item in _upgradeDisableObjects)
            {
                item.SetActive(true);
            }
            
            foreach (var item in _upgradeDisableObjects)
            {
                if (item)
                {
                    item.SetActive(true);
                }
            }
            
            gameObject.transform.parent.gameObject.SetActive(false);
        });
        
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

interface ISelectableManager
{
    public Data2048 Data { get; }
}