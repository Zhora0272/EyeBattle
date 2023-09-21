using UnityEngine;

public class PlayerBattleParticipant : BaseBattleParticipant
{
    [SerializeField] private EyePlayerController _playerController;

    private void Awake() => EyeParameters = _playerController;
    
}