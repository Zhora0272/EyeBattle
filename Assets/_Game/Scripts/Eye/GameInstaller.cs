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
            .FromComponentInNewPrefab(EnemyBotPrefab)
            .UnderTransform(_enemyPoolTransform).WhenInjectedInto<BotSpawnManager>();

        Container.BindMemoryPool<MoveableBattleParticipantBaseController, BattleTeammateParticipantPooling>()
            .FromComponentInNewPrefab(TeammateBotPrefab)
            .UnderTransform(_teammatePoolTransform).WhenInjectedInto<BotSpawnManager>();
    }
}