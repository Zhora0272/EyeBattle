using Object = UnityEngine.Object;
using System.Collections.Generic;
using System;

namespace Pooling
{
    public abstract class BasePooling<T> where T : Enum
    {
        private Dictionary<T, List<IPoolingMono>> _poolElements;

        protected BasePooling()
        {
            _poolElements = new Dictionary<T, List<IPoolingMono>>();
        }

        public void Reset()
        {
            _poolElements.Clear();
        }

        public IPoolingMono GetPoolElement(T type, IPoolingMono decorElement)
        {
            var state = _poolElements.TryGetValue(type, out var result);

            if (!state)
            {
                result = new List<IPoolingMono>();
                _poolElements.Add(type, result);
            }

            for (int i = 0; i < result.Count; i++)
            {
                if (!result[i].PoolMonoObj)
                {
                    result.Remove(result[i]);
                    continue;
                }

                if (!result[i].PoolMonoObj.gameObject.activeInHierarchy)
                {
                    result[i].PoolActivate();
                    return result[i];
                }
            }

            var element = Object.Instantiate(decorElement.PoolMonoObj) as IPoolingMono;

            result.Add(element);
            return element;
        }
    }
}

