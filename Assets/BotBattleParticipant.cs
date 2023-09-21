using UnityEngine;

public class BotBattleParticipant : BaseBattleParticipant
{
    [SerializeField] private EyeBaseController _eyeBaseController;
    private void Awake() => EyeParameters = _eyeBaseController;

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