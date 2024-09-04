using UnityEngine;

public abstract class BaseBattleParticipant : MonoBehaviour
{
    [SerializeField] private bool _registerState;
    [SerializeField] private bool _canRegister;
    
    public INpcParameters npcParameters;

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
        _manager.Register(npcParameters);
    }

    protected void UnRegister()
    {
        if(!_canRegister) return;
        
        _registerState = false;
        _manager.UnRegister(npcParameters);
    }
}