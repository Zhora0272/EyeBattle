# Queue System
The idea behind the Queue System is to be able to display screens based on a priority.
The lowest number being the highest priority.  This way we can control which popup is 
shown to the user first if we are required to show a series of popups.

## How to use:
Add to your installer
```c#
FromNewComponent<ScreenQueueManager>();
```
Create a new ScreenTask which has 2 **optional** parameters:
1. Priority.  ***0 is the Highest Priority.***  If no priority is set, it will default to 1.
2. IScreenProperties.
```c#
var noCoinsTask = new ScreenTask<NoCoinsPopup>(0, new TestProps(1,"test"));
```

When you want them to be displayed, you need to Add them to the Queue.  
If tasks have the same priority, they will be displayed to the user in the order they were added (FIFO).

```c#
_screenQueueManager.AddToQueue(noCoinsTask);
```

So a test class would look like this if you created multiple tasks:
```c#
public class Test
{
    [Inject] private ScreenQueueManager _screenQueueManager;
    
    void Start()
    {
        var noCoinsTask = new ScreenTask<NoCoinsPopup>(0, new TestProps(1,"test"));
        var languageTask = new ScreenTask<LangaugePopup>();
        _screenQueueManager.AddToQueue(noCoinsTask);
        _screenQueueManager.AddToQueue(languageTask);
    }
}
```

## How to see it working

At Runtime after you've added your series of popups to the queue:
* Go to [UIFrame] > Popups in the hierarchy
* You will see only the highest priority 
* Once you close that screen, the next screen you added to the queue will be enabled.

