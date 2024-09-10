using UnityEngine;
using Zenject;

public class ScreenInputInstaller : MonoInstaller
{
    [SerializeField] private ScreenInputEventView screenInputEventView;
    private ScreenInputEventController _screenInputEventController;
    
    [SerializeField] private ScreenToWorldCastView screenToWorldCastView;

    public override void InstallBindings()
    {
        //controller
        Container.Bind<ScreenInputEventController>().AsSingle();
        Container.Bind<ScreenInputEventView>().FromInstance(screenInputEventView).AsSingle().NonLazy();
        
        //view
        Container.BindInterfacesAndSelfTo<ScreenToWorldCastView>().FromInstance(screenToWorldCastView).AsSingle();
        Container.BindInterfacesAndSelfTo<ScreenToWorldCastController>().AsSingle();

    }
}
