using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class BotBattleParticipant : BaseBattleParticipant
{
    [FormerlySerializedAs("_eyeBaseController")] [SerializeField] private NpcBaseController npcBaseController;

    private void Awake()
    {
        npcParameters = npcBaseController;

        npcBaseController.IsDeath.Subscribe(value =>
        {
            if (value)
            {
                UnRegister();
            }

        }).AddTo(this);
    }

    public bool GetClosestElement(out INpcParameters param)
    {
        if (_manager.GetClosest(npcParameters, out var result))
        {
            param = result;
            return true;
        }

        param = null;
        return false;
    }
    
    
}