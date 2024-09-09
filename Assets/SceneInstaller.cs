using Bot.BotController;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private ScreenInputEventView screenInputEventView;
    private ScreenInputEventController _screenInputEventController;
    
    [SerializeField] private BotCommandView _botCommandView;
    private BotCommandController _botCommandController;

    [SerializeField] private ScreenToWorldCastView screenToWorldCastView;

    [SerializeField] private BotSpawnManager _botSpawnManager;
    
    public override void InstallBindings()
    {
        Container.Bind<BotCommandController>().AsSingle();
        Container.Bind<ScreenInputEventController>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScreenToWorldCastController>().AsSingle();
        
        Container.Bind<ScreenInputEventView>().FromInstance(screenInputEventView).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScreenToWorldCastView>().FromInstance(screenToWorldCastView).AsSingle();
        Container.Bind<BotCommandView>().FromInstance(_botCommandView).AsSingle().NonLazy();
        Container.BindInterfacesTo<BotSpawnManager>().FromInstance(_botSpawnManager).AsSingle();
    }
}
