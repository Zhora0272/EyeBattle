using Bot.BotController;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private SelectionSystemView _selectionSystemView;
    private SelectionSystemController _selectionSystemController;
    
    [SerializeField] private BotCommandView _botCommandView;
    private BotCommandController _botCommandController;

    [SerializeField] private BotSpawnManager _botSpawnManager;
    
    public override void InstallBindings()
    {
        Container.Bind<SelectionSystemView>().FromInstance(_selectionSystemView).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SelectionSystemController>().AsSingle();
        
        Container.Bind<BotCommandView>().FromInstance(_botCommandView).AsSingle().NonLazy();
        Container.Bind<BotCommandController>().AsSingle();

        Container.BindInterfacesTo<BotSpawnManager>().FromInstance(_botSpawnManager).AsSingle();
    }
}
