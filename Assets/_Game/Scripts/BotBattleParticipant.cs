using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class BotBattleParticipant : BaseBattleParticipant
{
    [FormerlySerializedAs("movableBattleParticipantBaseController")] [SerializeField] private MoveableBattleParticipantBaseController moveableBattleParticipantBaseController;

    private void Awake()
    {
        battleParticipantParameters = moveableBattleParticipantBaseController;

        moveableBattleParticipantBaseController.IsDeath.Subscribe(value =>
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