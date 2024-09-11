using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private MoveableBattleParticipantBaseController EnemyBotPrefab;
    [SerializeField] private MoveableBattleParticipantBaseController TeammateBotPrefab;

    [SerializeField] private Transform _teammatePoolTransform;
    [SerializeField] private Transform _enemyPoolTransform;

    public override void InstallBindings()
    {
        Container.BindMemoryPool<MoveableBattleParticipantBaseController, BattleEnemyParticipantPooling>()
            .WithInitialSize(20)
            .FromComponentInNewPrefab(EnemyBotPrefab)
            .UnderTransform(_enemyPoolTransform);
        
        Container.BindMemoryPool<MoveableBattleParticipantBaseController, BattleTeammateParticipantPooling>()
            .WithInitialSize(20)
            .FromComponentInNewPrefab(TeammateBotPrefab)
            .UnderTransform(_teammatePoolTransform);
    }
}

public class BattleEnemyParticipantPooling : BaseBattleParticipantPooling<MoveableBattleParticipantBaseController>
{
        
}
public class BattleTeammateParticipantPooling : BaseBattleParticipantPooling<MoveableBattleParticipantBaseController>
{
        
}