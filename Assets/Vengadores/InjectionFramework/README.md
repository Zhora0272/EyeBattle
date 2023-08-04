# Injection Framework

This framework provides field injections for C# classes and MonoBehaviours.

## Basic usage
Create a script named `MainSceneInstaller` and use the `Installer` base class. Create an empty game object in MainScene and attach `MainSceneInstaller` to it.
```c#
public class MainSceneInstaller : Installer
{
    protected override void Setup()
    {
        FromNew<UpgradeManager>();
        FromNew<ConfigManager>();
        FromNewComponent<ThemeManager>();
        ... 
    }
}
```
Add `Inject` attribute to fields to get instances.
```c#
public class UpgradeManager
{
    [Inject] private ConfigManager _configManager;
    [Inject] private ThemeManager _themeManager;
    
    ...
}
```

## Installers

Installers are the entry points for the definitions of the bindings. 
These definitions are used for injecting fields with `[Inject]` attribute. 
Add bindings by overriding the `Setup()` function. 
Installers are Monobehaviours, so you need to attached them to a GameObject.

There are two types of Installer: SceneInstaller and ProjectInstaller

### SceneInstaller

Scene installers are related to their scene. 
When the scene gets loaded, the Installer starts the setup process - defined in the `Setup()` function.
When the scene gets destroyed, created instances will be removed from the DiContainer. 
So the objects created by Installer will live in scene scope.

To create a scene installer:
- Create a script and inherit it from `Installer`
- Override the `Setup()` function
- Attach your script to a GameObject
- Put your game object to a scene



**IMPORTANT:** 
Installers are using `Awake` methods. You need to make sure the installer is the first thing to receive it. 
The general rule is not to use `Awake` methods in the game at all, if possible. Use the `Start` method for initializing.
This way, we know that injections are ready in the `Start` method and further.

### ProjectInstaller

Since SceneInstallers are for individual scenes, ProjectInstaller is for global bindings across all the scenes.
It is optional, and it lives in `DontDestroyOnLoad` scope.
It is perfect for binding common objects for every scene.

Using ProjectInstaller for every common bindings will make each scene playable without depending on other scenes, so you can skip loading scenes or menus during development to jump to other scenes directly.

To create a project installer:
- Create a script and inherit it from `Installer`
- Override the `Setup()` function
- Attach your script to a GameObject
- Save the GameObject as a Prefab under `Resources` folder
- Rename the prefab to `ProjectInstaller`

When the game loads and first scene installer starts to setup, it will search for ProjectInstaller. If it is found, project installer will setup first.

### Installer Setup

When you override `Setup()` function, you will have access to a couple of binding methods.

#### FromNew\<T>

`FromNew` is used for non-monobehaviour objects.
It creates the instance of the type using ` = new` operator and binds to the container.
Optional `Id` parameter is used for accessing when multiple instances exist in the container.


#### FromNewComponent\<T>

`FromNewComponent` is used for monobehaviour objects.
It creates the component on the installer gameObject using `AddComponent<T>` and binds to the container.
Optional `Id` parameter is used for accessing when multiple instances exist in the container.

#### BindObject\<T>

`BindObject` is used for binding existing objects to the container.
Optional `Id` parameter is used for accessing when multiple instances exist in the container.

### Inject Attribute

`[Inject]` is used on private fields of a class to get instances from DiContainer. 
It also works for interfaces.
It has an optional `Id` parameter, which can be used when multiple instances of the same type exists.
If the field type is array, it will inject an array with all the instances for that type.

### Setup Example

```c#
public interface ICostCalculator {...}

public class CoinCalculator : ICalculator {...}
public class XPCalculator : ICalculator {...}
```
```c#
public class EnergyManager 
{
    [Inject] private UpgradeManager _upgradeManager;
    [Inject] private NakamaManager _nakamaManager;
    [Inject] private ICalculator[] _calculators;
    ...
}
```
```c#
public class UpgradeManager : MonoBehaviour
{
    [Inject("Coin")] private ICalculator _coinCalculator;
    [Inject("Xp")] private ICalculator _xpCalculator;
    ...
}
```
```c#
public class MainSceneInstaller : Installer
{   
    protected override void Setup()
    {
        FromNew<EnergyManager>();
        
        FromNewComponent<UpgradeManager>();
        
        FromNew<CoinCalculator>("Coin");
        FromNew<XPCalculator>("Xp");
        ... 
    }
}
```
```c#
public class ProjectInstaller : Installer
{   
    public NakamaManager NakamaPrefab;
    
    protected override void Setup()
    {
        NakamaManager nakama = Instantiate(NakamaPrefab);
        BindObject(nakama);
        ... 
    }
}
```
## IInitializable/IDisposable

When using monobehaviours, we know the injections are ready in `Start()` method because all the injections are made in the installers `Awake()`.
However, when we are using non-monobehaviour object instances we don't have access to `Start()` method. 
If we want to know when injections are ready for non-monobehavior instances, we need to implement `IInitializable` interface.
This interface will come with `Init()` method, which gets called when injections are ready.

The same rule applies for the `OnDestroy()` method. 
If we want to know when a non-monobehaviour instance is destroyed, we need to implement `IDisposable` interface.
This interface will come with `Dispose()` method, which gets called when the related installer gets destroyed.

`Init()` only gets called during Setup. It is NOT used with runtime injections.

`Dispose()` only gets called if the scene is unloaded.

The best practice is implementing `IInitializable` to both monobehaviour and non-monobehaviour instances to have a consistent codebase.

```c#
public class UpgradeManager : MonoBehaviour, IInitializable, IDisposable
{
    [Inject] private SignalHub _signalHub;
    ...
    
    public void Init()
    {
        _signalHub.Get<LevelChangedSignal>().AddListener(this, OnLevelChanged);
    }
    
    public void Dispose()
    {
        _signalHub.RemoveAllListeners(this);
    }
}
```

## Runtime Binding and Injection

`DiContainer` has all the instance references, and it is used for injecting fields with `[Inject]` attribute.
Instances created by the installers are automatically injected. 
However, if you use `Instantiate()` or `new` operator, the injection framework don't know about these objects.
For these objects, binding and injection has to be done with `DiContainer`.

***Important***: Runtime binding and injection is not supported during installer setup.

`DiContainer` has a reference to itself, so that you can get it with injection anytime.

```c#
[Inject] private DiContainer _diContainer;
```

### DiContainer.CreateFromNew\<T>

Similar to `Installer.FromNew<T>`; it will create an instance, bind it to the diContainer and inject on the fields of the instance.
Optional id parameter can be used.

```c#
var playingState = _diContainer.CreateFromNew<PlayingState>();
```

### DiContainer.CreateFromNewComponent\<T>

Similar to `Installer.CreateFromNewComponent<T>`; it will create a component on a gameObject, bind it to the diContainer and inject on the fields of the instance.
Optional id parameter can be used.

```c#
var controller = _diContainer.CreateFromNewComponent<CharacterAnimController>(characterRoot);
```

### DiContainer.Inject

If you already have an instance and don't want to bind it to DiContainer, you can use this to inject instances on the fields.

```c#
var playingState = new PlayingState();
_diContainer.Inject(playingState);
```

### DiContainer.RegisterInstance

If you already have an instance and you want it to be accessible via injections, you can register it using this method.
If you want to do the same in installers, you should use `Installer.BindObject<T>`.

```c#
_diContainer.RegisterInstance(instance);

...

public class UpgradeManager
{
    [Inject] private LocalizationManager _localizationManager;
    ...
}
```

### DiContainer.Get\<T>

If you want to get an instance from DiContainer (similar to using `[Inject]` attribute), you can use this method to get the instance.
If there is multiple instance for a given type, the first instance will be returned. For multiple instances, you can use ids.

```c#
var playingState = _diContainer.Get<PlayingState>();

Canvas canvas = _diContainer.Get<Canvas>("Main");
```

### DiContainer.GetAll\<T>

If you want to get all the instances for a type from DiContainer (similar to using `[Inject]` attribute on an array), you can use this method.

```c#
Canvas[] canvases = _diContainer.GetAll<Canvas>();
```

## AutoInjector

If a monoBehaviour uses `[Inject]` attribute, it needs to be injected. 
If a gameObject is created with `Instantiate`, injection framework don't know anything about it.
There are two ways of injecting a created MonoBehaviour.

- The first way is using runtime injection. However, you need to do it for every component that needs to be injected.
```c#
var chestPopupObj = Instantiate(_chestPopupPrefab);
var chestPopup = chestPopupObj.GetComponent<ChestPopup>();
_diContainer.Inject(chestPopup);
```

- The second and easier way is to attach an `[AutoInjector]` component to the prefab root. 
It will scan each component in the prefab, including children, and injects to fields. No scripting is needed.

## Binding Component

Sometimes we want to leave gameObjects in the scene. 
If we want a component on them to be accessible via DiContainer, we can attach a `Binding` component to them.
For example, leaving a canvas in the scene, attaching `Binding` component to it, and later getting it via injection:

```c#
[Inject("Main")] private Canvas _mainCanvas;

OR

_mainCanvas = _diContainer.Get<Canvas>("Main");
```

***Important***: `Binding` component is only used during the **Setup** phase. It is not supported on prefabs.




## Lifecycle of the Injection Framework

- Scene starts to load
  - Unity calls all Awake() functions for components in the scene
  - `SceneInstaller.Awake()` in the scene is called
  - Installer tries to find `ProjectInstaller.prefab` under `/Resources`
    - If `ProjectInstaller.prefab` is found, it is Instantiated and moved to `DontDestroyOnLoad` scene
    - `ProjectInstaller.Awake()` is called upon instantiating
    - `ProjectInstaller.Setup()` is called, instances added to injection queue
    - Setup complete, injections are made for the `ProjectInstaller` from the queue
    - `Init()` methods called for `IInitializable` implemented instances in the ProjectInstaller
  - `SceneInstaller.Setup()` in the scene is called, instances added to injection queue
  - Installer scans the scene for `Binding` components and binds them
  - All the monobehaviours are added to injection queue
  - Setup complete, injections are made for the `SceneInstaller` from the queue
  - `Init()` methods called for `IInitializable` implemented instances in the SceneInstaller


- Scene unloaded
  - `SceneInstaller.OnDestroy()` in the scene is called
  - `Dispose()` methods called for `IDisposable` implemented instances in the SceneInstaller


- App quit
  - `ProjectInstaller.OnDestroy()` in the scene is called
  - `Dispose()` methods called for `IDisposable` implemented instances in the ProjectInstaller
