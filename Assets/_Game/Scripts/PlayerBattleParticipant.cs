using UniRx;
using UnityEngine;

public class PlayerBattleParticipant : BaseBattleParticipant
{
    [SerializeField] private NpcPlayerController _playerController;

    private void Awake() => npcParameters = _playerController;

    public override void Start()
    {
        base.Start();
        _playerController.IsDeath.Subscribe(value =>
        {
            if (value)
            {
                //UnRegister();
            }
        }).AddTo(this);
    }
}