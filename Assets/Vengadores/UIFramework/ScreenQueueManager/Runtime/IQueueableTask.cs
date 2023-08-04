using System;

namespace Vengadores.UIFramework.ScreenQueueManager
{
    public interface IQueueableTask
    {
        int Priority { get; }
        void Execute();
        event Action<IQueueableTask> OnComplete;
    }
}
