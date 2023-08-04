# Utilities

## Scene Auto Loader

This utility adds a `Voodoo > Scene Autoload` menu containing options to select a "master scene", enable it to be auto-loaded when the user presses play in the editor. 
When enabled, the selected scene will be loaded on play, then the original scene will be reloaded on stop.

First, select a master scene to load on play
`Voodoo > Scene Autoload > Select Master Scene`

Then enable `Voodoo > Scene Autoload > Load Master on Play`

## Empty Folder Remover

This utility adds a `Voodoo > Clean Empty Folders` window to find empty folders int the project.

Click `Find Empty Dirs` in the window.
If empty folders found, paths of the folders will be listed in the window.

Click to `Delete All` button to delete listed empty folders.

## Mesh Renderer Sorting

This utility exposes the Sorting Layer / Order in MeshRenderer since it's there but not displayed in the inspector.

## TickableManager

If you want to have `Update()` or `LateUpdate()` and avoid weight of MonoBehaviours, 
you can use `TickSignal` and `LateTickSignal` in your non-MonoBehaviour classes.

Install `TickableManager`
```c#
FromNewComponent<TickableManager>();
```

Subscribe to `TickSignal` or `LateTickSignal` through `SignalHub`
```c#
[Inject] private SignalHub _signalHub;

...

_signalHub.Get<TickSignal>().AddListener(this, OnTick);
_signalHub.Get<LateTickSignal>().AddListener(this, OnLateTick);

...

private void OnTick() {...}
private void OnLateTick() {...}
```

You can also start/stop ticking. It will start ticking by default.
```c#
[Inject] private TickableManager _tickableManager;

...

_tickableManager.StartTicking();
_tickableManager.StopTicking();
```