using System;
using UnityEngine.Events;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine;
using UniRx;

public class MergeTutorial : MonoBehaviour
{
    [SerializeField] private MergeController _mergeController;
    
    [SerializeField] private GameObject _lockbackground;
    [SerializeField] public Button _buyButton;
    [SerializeField] private Button _fightButton;

    [SerializeField] private GameObject _gridTable;
    [SerializeField] private GameObject _tutorialInfinity;
    
    [SerializeField] private CinemachineVirtualCamera _camera;

    [SerializeField] private RectTransform _buyButtonHandAnimation;
    [SerializeField] private RectTransform _gridHandleAnimation;
    
    private void Start()
    {
        IDisposable _infinityTutorialDisposable = null;
        
        /*var index = PlayerPrefs.GetInt(PlayerPrefsEnum.AllWinLevelCount .ToString());
        if (index == 0)
        {
            _buyButton.transform.parent.gameObject.SetActive(false);
            _gridTable.SetActive(false);
        }
        else if (index == 1)
        {
            if(PlayerPrefs.GetInt(PlayerPrefsEnum.TutorialStep1.ToString()) == 0)
            {
                OpenTutorialStep1();
            }
        }*/

        /*if (index < 1)
        {
            _tutorialInfinity.SetActive(true);
            
            _infinityTutorialDisposable = _inputController.ShootingTabStream.Subscribe(_ =>
            {
                _tutorialInfinity.SetActive(false);
                _infinityTutorialDisposable.Dispose();
            }).AddTo(this);
        }*/
    }

    private UnityAction closeStep1Action;
    private void OpenTutorialStep1()
    {
        _buyButtonHandAnimation.gameObject.SetActive(true);
        _fightButton.transform.parent.gameObject.SetActive(false);
        _lockbackground.SetActive(true);
        
        //
        closeStep1Action = CloseTutorialStep1;
        _buyButton.onClick.AddListener(() =>
        {
            closeStep1Action.Invoke();
            _buyButton.onClick.RemoveListener(closeStep1Action);
        });
        //
    }
    
    private void CloseTutorialStep1()
    {
       /* PlayerPrefs.SetInt(PlayerPrefsEnum.TutorialStep1.ToString(), 1);
        _buyButtonHandAnimation.gameObject.SetActive(false);
        _lockbackground.SetActive(false);

        if (PlayerPrefs.GetInt(PlayerPrefsEnum.TutorialStep2.ToString()) == 0)
        {
            OpenTutorialStep2();
        }*/
    }
    
    
    private void OpenTutorialStep2()
    {
        _gridHandleAnimation.gameObject.SetActive(true);
        _mergeController.MaxWeaponLevel.Subscribe(value =>
        {
            if (value > 0)
            {
                CloseTutorialStep2();
            }
        }).AddTo(this);
    }
    
    private void CloseTutorialStep2()
    {
       /* PlayerPrefs.SetInt(PlayerPrefsEnum.TutorialStep2.ToString(), 1);
        
        _gridHandleAnimation.gameObject.SetActive(false);
        _fightButton.transform.parent.gameObject.SetActive(true);*/
    }
}
