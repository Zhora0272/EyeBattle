using UnityEngine;

public class BaseBattleParticipant : MonoBehaviour
{
    public IEyeParameters EyeParameters;
    
    protected BattleParticipantsManager _manager;

    public virtual void Start()
    {
        _manager = MainManager.GetManager<BattleParticipantsManager>();

        InitAfterManagerInit();
    }

    private void InitAfterManagerInit()
    {
        _manager.Register(EyeParameters);
    }

    private void OnDisable()
    {
        _manager.UnRegister(EyeParameters);
    }
}