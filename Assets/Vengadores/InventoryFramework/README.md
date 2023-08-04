# Inventory Framework

## Using Base Game and want to 

If you're using the base game and simply want to extend it, here's how:
1. To InventoryTypes.cs add your new constant. 
```c#
  public const string MyNewItem = "foobar";
```
2. To InventoryHandler.cs define the starting amount and a clamp function for your new item.  By default things start at zero and are clamped between 0 and maxint.  If this works for your item no action needed.
3. Start using it:
```c#
  // use the injector to get some references. 
  [Inject] private InventoryManager _inventoryManager;
  [Inject] private SignalHub _signalHub;
...
  // so you can update the UI 
  _signalHub.Get<InventoryChangedSignal>().AddListener(this, OnInventoryChanged);
...
  // Update the UI if needed.
  private void OnInventoryChanged(InventoryChangeInfo obj)
  {
    if (obj.Type == InventoryTypes.MyNewItem)
    {
      UpdateUIWithNewAmount(obj.CurrentAmount);
    }
  }
...
  // to get the current value 
  var x = _inventoryManager.Get(InventoryTypes.MyNewItem);
  // to adjust the value 
  _inventoryManager.AddAndSync(InventoryTypes.MyNewItem, delta);
```
## Basic usage

Implement `IInventoryHandler`

```c#
public class InventoryHandler : IInventoryHandler
{
    public int GetInitialAmount(string type) { ... }

    public int Clamp(string type, int amount) { ... }
}
```

Install `IInventoryHandler`, `InventoryData` and `InventoryManager`
```c#
FromNew<InventoryManager>();
FromNew<InventoryHandler>();
FromNew<InventoryData>();
```

Initialize `InventoryManager`
```c#
_inventoryManager.Initialize();
```

Get a value

```c#
var coins = _inventoryManager.Get("Coin");
```

Add an amount

```c#
_inventoryManager.AddAndSync("Coin", 100);
```

Commit and push an amount
```c#
var commit = _inventoryManager.Commit("Level", 1);

_inventoryManager.Push(commit);
```

## Inventory Handler

Inventory framework needs an implementation of `IInventoryHandler` in order to get configs.

`GetInitialAmount(type)` is used for getting default values of an inventory type.
For example, most of the games start with 0 coins but the level starts from 1.

`Clamp(type, amount)` is used for applying limits to inventory contents. 
For example you may want to limit coins to 1m but boosters to 100.

### Example implementation of IInventoryHandler
```c#
public class InventoryHandler : IInventoryHandler
{
    private const string PlayerPrefsPath = "InventoryModel";

    public int GetInitialAmount(string type)
    {
        switch (type)
        {
            case "Coin": return 1000;
            case "Gem": return 10;
            default: return 0;
        }
    }

    public int Clamp(string type, int amount)
    {
        switch (type)
        {
            case "Coin": return Mathf.Clamp(amount, 0, 1000000);
            case "Gem": return Mathf.Clamp(amount, 0, 10000);
            default: return amount;
        }
    }
}
```

## Setup

`InventoryManager` and the `IInventoryHandler` implementation needs to be installed.
```c#
FromNew<InventoryHandler>();
FromNew<InventoryManager>();
```

After installing, you need to call `Initialize()` method on `InventoryManager`.
This will trigger data loading, so if you are using a database you need make sure it is ready.

```c#
_inventoryManager.Initialize();
```

## Transactions

When we want to make a change on the inventory (adding coins etc.) we need to handle 3 things:
- Update the runtime model
- Save the data 
- Refresh UI views

But the problem is when we add an animation (doobers for example), we can't do all of them at the same time. 
We need to wait animations to finish (waiting a doober to reach its target etc.) to refresh UI Views. 
During the animation, user can simply quit the app and it may result in critical loss of data.

So the execution order should be:
- Update the runtime model
- Save the data
- Wait for the doober to reach its target
- Refresh the UI views

But what happens if some other UI view or game logic gets the value the doober is still flying?
User is still seeing the old value because UI View is not refreshed, but we already updated runtime model. 

To solve that issue, Inventory Framework uses a staged approach for transactions. 
This approach contains `Commit` and `Push` methods just like git and utilizes `Signals` or `Actions` for refreshing the UI.

So in this framework, execution order is:
- Commit a change to the inventory
  - Data saved
  - Runtime model not updated
- Wait for the doober to reach its target
- Push the change
  - Runtime model updated
  - InventoryChangedSignal dispatched
  - Registered Actions Invoked
- Refresh UI views when InventoryChangedSignal dispatched

### Inventory.Commit

Inventory framework uses two models internally. 
When you `Commit` a change, it will update the primary runtime model and save the change immediately.

However if you get the value from the inventory, it will give you the old value. 
In other words, committed changes are not visible to game until they are pushed.

If the user quits the app after the `Commit`, data is already saved. 
So opening the app again will load the data but with the all committed data. 

Returned object is used for pushing it later.

```c#
// Gem is 0 here

var commit = _inventoryManager.Commit("Gem", 10);

// Gem is still 0 here
```

### Inventory.Push

When you push a commit, it will update the secondary runtime model and dispatch `InventoryChangedSignal`.
So the change you made is now visible by all the game.

```c#
// Gem is 0 here

_inventoryManager.Push(commit);

// Gem is 10 here
```

### Inventory.LazyAdd

If you don't use animations and there is no need for a staging, you can use `LazyAdd` shortcut to change a value immediately.
It is basically commiting a change and pushing it right after.

```c#
// Gem is 0 here

_inventoryManager.LazyAdd("Gem", 10);

// Gem is 10 here
```

### Inventory.Get

To get the value of an inventory type, use this method. 
It will use secondary runtime model, so committed changes are not visible until they are pushed.

```c#
var gems = _inventoryManager.Get("Gem");
```

### Example transaction

```c#
public class Example
{
    [Inject] InventoryManager _inventoryManager;
    [Inject] Doobers _doobers;
  
    public void GiveCoins()
    {
        var commit = _inventoryManager.Commit(InventoryTypes.Coin, 100);
        
        _doobers.CreateCoinDoober(() => {
            _inventoryManager.Push(commit);
        });
    }
}


public class CoinView : MonoBehaviour
{
    [Inject] SignalHub _signalHub;
    
    private void Start()
    {
        _signalHub.Get<InventoryChangedSignal>().AddListener(this, OnInventoryChanged);
    }
    
    private void OnInventoryChanged(InventoryChangeInfo info)
    {
        if(info.Type == InventoryTypes.Coin) 
        {
            // Refresh
        } 
    }
}

// alternative way to register for just your type of inventory item.
public class CoinView : MonoBehaviour
{
    [Inject] InventoryManager _inventory;
    
    private void Start()
    {
        _inventory.AddOnChangeListener(InventoryTypes.Coin,OnGoldChanged);
    }
    
    private void OnGoldChanged(InventoryChangeInfo info)
    {
        // will only be called for Coin type changes. 
        // Refresh
    }
}
```

## Extending the InventoryManager

Using strings for the types is not error-prone. 
So creating a class for constants is a good way to minimize the type errors.

```c#
public static class InventoryTypes
{
    public const string Coin = "Coin";
    public const string Gem = "Gem";
}
```
```c#
var coins = _inventoryManager.Get(InventoryTypes.Coin);
```

Another good way is extending the inventory class for common cases like getting coins.
```c#
public static class InventoryExtensions
{
    public static int GetCoins(this InventoryManager inventoryManager)
    {
        return inventoryManager.Get(InventoryTypes.Coin);
    }
}
```
```c#
var coins = _inventoryManager.GetCoins();
```