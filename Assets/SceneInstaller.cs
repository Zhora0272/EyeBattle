using Bot.BotController;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private BotCommandView _botCommandView;
    private BotCommandController _botCommandController;

    [SerializeField] private BotSpawnManager _botSpawnManager;

    [SerializeField] private SanctuaryView _sanctuaryView;

    public override void InstallBindings()
    {
        //controller
        Container.Bind<BotCommandController>().AsSingle();
        Container.Bind<SanctuaryController>().AsSingle();

        //view
        Container.Bind<SanctuaryView>().FromInstance(_sanctuaryView).AsSingle();
        Container.Bind<BotCommandView>().FromInstance(_botCommandView).AsSingle().NonLazy();
        Container.BindInterfacesTo<BotSpawnManager>().FromInstance(_botSpawnManager).AsSingle();
    }
}