using UnityEngine;

public class BaseBattleParticipant : MonoBehaviour
{
    [SerializeField] private bool _registerState;
    [SerializeField] private bool _canRegister;
    
    public IEyeParameters EyeParameters;

    protected BattleParticipantsManager _manager;


    public virtual void Start()
    {
        _manager = MainManager.GetManager<BattleParticipantsManager>();

        InitAfterManagerInit();
    }

    private void InitAfterManagerInit()
    {
        if(!_canRegister) return;
        
        _registerState = true;
        _manager.Register(EyeParameters);
    }

    protected void UnRegister()
    {
        if(!_canRegister) return;
        
        _registerState = false;
        _manager.UnRegister(EyeParameters);
    }
}