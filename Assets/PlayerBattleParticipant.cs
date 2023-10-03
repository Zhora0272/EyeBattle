using UniRx;
using UnityEngine;

public class PlayerBattleParticipant : BaseBattleParticipant
{
    [SerializeField] private EyePlayerController _playerController;

    private void Awake() => EyeParameters = _playerController;

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