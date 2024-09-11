using System;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private MoveableBattleParticipantBaseController EnemyBotPrefab;
    [SerializeField] private MoveableBattleParticipantBaseController TeammateBotPrefab;

    [SerializeField] private BarracksView[] _barracksView;

    [SerializeField] private Transform _teammatePoolTransform;
    [SerializeField] private Transform _enemyPoolTransform;

    public override void InstallBindings()
    {
        Container.Bind<BarracksController>().AsSingle();
        Container.Bind<BarracksView[]>().FromInstance(_barracksView);

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

[Serializable]
public class BattleEnemyParticipantPooling : BaseBattleParticipantPooling<MoveableBattleParticipantBaseController>
{
    
}
[Serializable]
public class BattleTeammateParticipantPooling : BaseBattleParticipantPooling<MoveableBattleParticipantBaseController>
{
        
}