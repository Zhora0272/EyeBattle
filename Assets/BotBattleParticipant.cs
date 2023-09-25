using UniRx;
using UnityEngine;

public class BotBattleParticipant : BaseBattleParticipant
{
    [SerializeField] private EyeBaseController _eyeBaseController;

    private void Awake()
    {
        EyeParameters = _eyeBaseController;

        _eyeBaseController.IsDeath.Subscribe(value =>
        {
            if (value)
            {
                UnRegister();
            }

        }).AddTo(this);
    }

    public bool GetClosestElement(out IEyeParameters param)
    {
        if (_manager.GetClosest(EyeParameters, out var result))
        {
            param = result;
            return true;
        }

        param = null;
        return false;
    }
    
    
}