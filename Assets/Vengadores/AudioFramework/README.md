# Audio Framework

## Basic Usage

Create `AudioDatabase` asset in `Resources` folder, add audio clips

Create `AudioManager` instance in an installer
```c#
FromNewComponent<AudioManager>();
```
Get the `AudioManager` with injection
```c#
[Inject] private AudioManager _audioManager;
```
Play sounds or music, turn on/off
```c#
_audioManager.PlayOneShot("Explosion");
_audioManager.PlayLoop("Fire");
_audioManager.TurnOffSounds();
_audioManager.TurnOnSounds();
```
```c#
_audioManager.PlayMusic("MainMusic");
_audioManager.TurnOffMusic();
_audioManager.TurnOnMusic();
```

## AudioDatabase

`AudioDatabase` is a scriptable object asset which is a collection of audio clips and their configs.

To create `AudioDatabase.asset`, right click on the project view and select `Create -> Voodoo -> AudioDatabase`.

`AudioDatabase` needs to be in `Resources` folder.

### AudioDatabase.InitialAudioPoolSize

Audio framework uses a `GameObjectPool` for creating audio source gameobjects. 
This config used for initial size of the pool.

### AudioDatabase.AudioList

AudioList is a collection of audio clips and their configs.
If you want to play an audio clip, you should add it to `AudioDatabase` first.

- **Volume**: It is the volume used by each audio clip, default is 1
- **Time Between**: It is a duration required to play the same audio clip again. 
- **Max Count**: Max Amount of the same audio clip to play at the same time. If set to 0, it is ignored.

## AudioManager

It is the main controller for all the audio. 

### AudioManager.PlayOneShot

You can play one shot audio clips with this method. It takes name of the audioClip in the `AudioDatabase`.
You can pass an optional pitch value. For example, it can be used for tap combos.

```c#
_audioManager.PlayOneShot("Explosion");

_audioManager.PlayOneShot("Upgrade", 0.9f);
```

### AudioManager.PlayLoop

Same as `PlayOneShot`, but it plays a **looping** audio clip.
It returns a `LoopingAudio` reference which can be used to stop.

```c#
var loopingAudio = _audioManager.PlayLoop("Fire");
---
loopingAudio.Stop();
```

### AudioManager.PlayMusic

You can play a background music with this method. It supports only one bg music to be played at the same time.

```c#
_audioManager.PlayMusic("BgMusic");
```

### AudioManager.TurnOnSounds / TurnOffSounds 

When you start the game, by default all the audio is muted. 
You need to turn them on manually by checking user settings data when your game initialized.

```c#
if(_settings.SoundEnabled) // this is a game impl.
{
    _audioManager.TurnOnSounds();
}
else
{
    _audioManager.TurnOffSounds();
}
```

### AudioManager.TurnOnMusic / TurnOffMusic

Same as sounds, you need to turn the music on manually by checking user settings data when your game initialized.

```c#
if(_settings.MusicEnabled) // this is a game impl.
{
    _audioManager.TurnOnMusic();
    _audioManager.PlayMusic("BgMusic");
}
else
{
    _audioManager.TurnOffMusic();
}
```

The parameter of `bool playLastMusic` in `TurnOnMusic` is used for runtime enabling/disabling. 
When it is true, it will try to start playing the last music automatically. 
If it is false, you need to play it manually by giving the name of the music.

```c#
_audioManager.TurnOnMusic(true);

OR

_audioManager.TurnOnMusic(false);
_audioManager.PlayMusic("AnotherBgMusic");
```

### AudioManager.IsSoundEnabled / IsMusicEnabled

Helper methods to check if sounds/music enabled.

### AudioManager.GetLastPlayTime

You can get last play time of an audio clip with this method. 
For example, you can use this time to adjust pitch value of a combo.

```c#
private int _tapCombo;

private void OnTapButtonClick()
{
    if (Time.realtimeSinceStartup - _audioManager.GetLastPlayTime("Tap") < 0.25f)
    {
        _tapCombo++;
    }
    else
    {
        _tapCombo = 0;
    }
    
    var pitch = Mathf.Lerp(1.0f, 1.3f, Mathf.Clamp01(_tapCombo / 10.0f));
    
    _audioManager.PlayOneShot("Tap", pitch);
}
```


