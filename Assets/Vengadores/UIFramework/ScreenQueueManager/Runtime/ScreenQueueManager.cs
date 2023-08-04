using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Vengadores.InjectionFramework;

namespace Vengadores.UIFramework.ScreenQueueManager
{
    public class ScreenQueueManager : IInitializable
    {
        [Inject] private DiContainer _diContainer = default;
        
        //private readonly SortedList _queue = new SortedList();
        public List<IQueueableTask> queue = new List<IQueueableTask>();

        private bool _inProgress;
        private const int ExecutionDelay = 3;

        public void AddToQueue(IQueueableTask task)
        {
            _diContainer?.Inject(task);

            AddSort(task);
        }

        public void AddSort(IQueueableTask task)
        {
            queue.Add(task);
            queue.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            if (_inProgress == false && queue.Count <= 1)
            {
                Delay();
            }
        }

        private async void Delay()
        {
            await Task.Delay(ExecutionDelay);
            Next();
        }

        private void Next(IQueueableTask completed = null)
        {
            if (completed != null)
                completed.OnComplete -= Next;

            if (queue.Count > 0)
            {
                _inProgress = true;

                var task = queue[0];
                queue.RemoveAt(0);
                
                task.OnComplete += Next;
                task.Execute();
            }
            else
            {
                _inProgress = false;
            }
        }

        public void Init()
        {
            //
        }
    }
}
