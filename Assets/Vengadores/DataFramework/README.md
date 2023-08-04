# Data Framework

## Basic Usage

Create a serializable class and inherit from `BaseData`.
```c#
[Serializable]
public class LanguageData : BaseData
{
    public string Language = "en";
}
```

Install `DataManager`, `PlayerPrefsDataHandler` and the data class.
```c#
FromNew<DataManager>();
FromNew<PlayerPrefsDataHandler>();

FromNew<LanguageData>();
```

Call `LoadAll` on `DataManager`
```c#
yield return _dataManager.LoadAll();
```

Get data instance with injections, modify it and save
```c#
[Inject] private LanguageData _languageData;

...

_languageData.Language = "de";
_languageData.Save();
```

## DataManager

After the install phase is completed, you need to `LoadAll` DataManager.
`LoadAll` will start loading all the `BaseData` models.
It returns an IEnumerator, so you can yield it for example, when loading the game scene.
```c#
yield return _dataManager.LoadAll();
```

## IDataHandler

It is used for implementing `Load` and `Save` operations.
Both operations need to invoke `onComplete` after they completed writing/reading.

Load operation is expected to **_populate_** given instance.

`PlayerPrefs` and `FileSystem` data handler is provided with the framework.

```c#
public class ExampleDataHandler : IDataHandler
{
    public void Load(BaseData data, Action onComplete)
    {
        // Load
        JsonConvert.PopulateObject(json, data);
        onComplete(data);
    }

    public void Save(BaseData data, Action onComplete)
    {
        // Save
        onComplete();
    }
}
```

## BaseData

### BaseData.OnLoaded
OnLoaded is called after when the data is loaded. 
```c#
public override void OnLoaded() { ... }
```

### BaseData.OnBeforeSave
OnBeforeSave is called before saving begins. It is useful for validating and preparing the data.
```c#
public override void OnBeforeSave() { ... }
```

### BaseData.OnSaved
OnSaved is called after saving completed.
```c#
public override void OnSaved() { ... }
```

### Example BaseData

```c#
[Serializable]
public class SettingsData : BaseData
{
    public bool Sound = true;
    public bool Music = true;
    public bool Notifications = true;
    public bool Vibrations = true;
    public string Name = "Master Chief";
    
    public override void OnLoaded()
    {
        //
    }
    
    public override void OnBeforeSave()
    {
        //
    }
    
    public override void OnSaved()
    {
        //
    }
}
```

