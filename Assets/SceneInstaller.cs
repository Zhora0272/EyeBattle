using Bot.BotController;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [FormerlySerializedAs("screenInputSystemView")] [FormerlySerializedAs("_selectionSystemView")] [SerializeField] private ScreenInputEventView screenInputEventView;
    private ScreenInputEventController _screenInputEventController;
    
    [SerializeField] private BotCommandView _botCommandView;
    private BotCommandController _botCommandController;

    [FormerlySerializedAs("screenToWorldColliderCastView")] [FormerlySerializedAs("_screenToWorldOperation")] [SerializeField] private ScreenToWorldCastView screenToWorldCastView;

    [SerializeField] private BotSpawnManager _botSpawnManager;
    
    public override void InstallBindings()
    {
        Container.Bind<ScreenInputEventView>().FromInstance(screenInputEventView).AsSingle().NonLazy();
        
        Container.BindInterfacesAndSelfTo<ScreenToWorldCastView>().FromInstance(screenToWorldCastView).AsSingle();
        Container.Bind<ScreenToWorldCastController>().AsSingle();
        
        Container.Bind<BotCommandView>().FromInstance(_botCommandView).AsSingle().NonLazy();
        Container.Bind<BotCommandController>().AsSingle();

        Container.BindInterfacesTo<BotSpawnManager>().FromInstance(_botSpawnManager).AsSingle();
    }
}
