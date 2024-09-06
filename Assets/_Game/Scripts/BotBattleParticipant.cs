using UniRx;
using UnityEngine;

public class BotBattleParticipant : BaseBattleParticipant
{
    [SerializeField] private MovableBattleParticipantBaseController movableBattleParticipantBaseController;

    private void Awake()
    {
        battleParticipantParameters = movableBattleParticipantBaseController;

        movableBattleParticipantBaseController.IsDeath.Subscribe(value =>
        {
            if (value)
            {
                UnRegister();
            }

        }).AddTo(this);
    }

    public bool GetClosestElement(out IBattleParticipantParameters param)
    {
        if (_manager.GetClosest(battleParticipantParameters, out var result))
        {
            param = result;
            return true;
        }

        param = null;
        return false;
    }
}