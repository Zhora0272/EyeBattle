using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class BotBattleParticipant : BaseBattleParticipant
{
    [FormerlySerializedAs("moveableBattleParticipantBaseController")] [FormerlySerializedAs("battleParticipantBaseController")] [FormerlySerializedAs("npcBaseController")] [SerializeField] private MovableBattleParticipantBaseController movableBattleParticipantBaseController;

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