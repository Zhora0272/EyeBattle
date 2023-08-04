# Signal Hub

## Basic usage

Create the `SignalHub` instance
```c#
var _signalHub = new SignalHub();
```
Define a signal class
```c#
public class NewRecordSignal : ASignal<long> { }
```

Subscribe or dispatch the signal
```c#
_signalHub.Get<NewRecordSignal>().Dispatch(newRecord);
```
```c#
_signalHub.Get<NewRecordSignal>().AddListener(this, OnNewRecord);
```
```c#
_signalHub.Get<NewRecordSignal>().RemoveListener(this);

OR

_signalHub.RemoveAllListeners(this);
```

## Signals

Quote from [Zenject Signal Doc](https://github.com/modesttree/Zenject/blob/master/Documentation/Signals.md):

> Signals are most appropriate as a communication mechanism when:
> - There might be multiple interested receivers listening to the signal
> - The sender doesn't need to get a result back from the receiver
> - The sender doesn't even really care if it gets received. In other words, the sender should not rely on some state changing when the signal is called for subsequent sender logic to work correctly. Ideally signals can be thought as "fire and forget" events
> - The sender triggers the signal infrequently or at unpredictable times
> 
> These are just rules of thumb, but useful to keep in mind when using signals. 
> The less logically coupled the sender is to the response behaviour of the receivers, the more appropriate it is compared to other forms of communication such as direct method calls, interfaces, C# event class members, etc.
> 
> When event driven program is abused, it is possible to find yourself in "callback hell" where events are triggering other events etc. and which make the entire system impossible to understand. 
> So signals in general should be used with caution.
> 
> Personally I like to use signals for high level game-wide events and then use other forms of communication (unirx streams, c# events, direct method calls, interfaces) for most other things.

### Signal Declaration

First, we need to declare the signal class. 
It is recommended to create a script file `GameSignals.cs` and put every signal class definition in it to keep all signals at one place.

You can create signal classes simply inheriting the `ASignal` base class.
```c#
public class LoadingCompleteSignal : ASignal { }
```

If you want to declare a signal which takes some parameters, you can use generic types.
If you have more than 3 parameters, you should combine them into a class.
```c#
public class QuestFinishedSignal : ASignal<int> { } // int for quest id
```

### Dispatching a Signal

To dispatch a signal, you can use `Dispatch` method on a signal reference. 
Signals are stored in a cache in `SignalHub`.
You can get a Signal from `SignalHub` using `Get` method.
You can pass parameters as well.

```c#
_signalHub.Get<LoadingCompleteSignal>().Dispatch();
```
```c#
_signalHub.Get<QuestFinishedSignal>().Dispatch(123);
```

### Listening a Signal

For listening a signal, you need to subscribe to it.
To subscribe a signal, you can use `AddListener` method on a signal reference. 
To unsubscribe, use `RemoveListener`.

You need to pass an object reference. This is used for bulk removing attached listeners.

```c#
_signalHub.Get<LoadingCompleteSignal>().AddListener(this, OnLoadingComplete);

...

private void OnLoadingComplete()
{
    ...
}
```
```c#
_signalHub.Get<QuestFinishedSignal>().AddListener(this, OnQuestFinished);

...

private void OnQuestFinished(int questId)
{
    ...
}
```

**IMPORTANT**: You need to make sure to not subscribe multiple times. 
It is important to remove listeners on counterparts of where you added them. 
You can use these to add/remove listeners:
- Start/Destroy
- OnEnable/OnDisable
- Init/Dispose (Injection Framework)
- OnCreated/OnDestroyed (UI Framework)
- OnOpening/OnClosing (UI Framework)

```c#
void Start()
{
    _signalHub.Get<LoadingCompleteSignal>().AddListener(this, OnLoadingComplete);
}

void OnDestroy()
{
    _signalHub.Get<LoadingCompleteSignal>().RemoveListener(this);
}
```

**IMPORTANT**: If you have two different classes listening the same signal, the order of the execution is not guaranteed.
If you are depending on the order, you can add another signal in between those calls and wait for it in the second class.

- `RewardManager` listens `QuestCompleteSignal`
- `CoinView` listens `QuestCompleteSignal`
- `QuestManager` dispatches `QuestCompleteSignal`


- RewardManager receives the signal, adds rewards
- CoinView receives the signal, shows coin value **with** added rewards.

OR

- CoinView receives the signal, shows coin value **without** added rewards.
- RewardManager receives the signal, adds rewards

To solve that, we can simply add another signal in between them:
- `RewardManager` listens `QuestCompleteSignal`
- `CoinView` listens `RewardClaimedSignal`
- `QuestManager` dispatches `QuestCompleteSignal`
- RewardManager receives `QuestCompleteSignal`, adds rewards and dispatches `RewardClaimedSignal`
- CoinView receives `RewardClaimedSignal` shows coin value **with** added rewards.


### Bulk removing all listeners

Most of the time, you will subscribe to multiple signals. 
To prevent missing `RemoveListener` calls, you can use `RemoveAllListeners` to remove attached listeners from an object. 

```c#
void Start()
{
    _signalHub.Get<LoadingCompleteSignal>().AddListener(this, OnLoadingComplete);
    _signalHub.Get<QuestFinishedSignal>().AddListener(this, OnQuestFinished);
    ...
}

void OnDestroy()
{
    _signalHub.RemoveAllListeners(this);
}
```