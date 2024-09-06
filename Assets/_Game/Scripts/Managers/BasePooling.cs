using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEngine;
using System;
using Bot.BotController;

public class BotPool : BasePooling<BotType, MovableBattleParticipantBotController>
{
    
}

public enum AmmoType
{
    Rocket,
}

public class AmmoPool : BasePooling<AmmoType, GunAmmoBase>
{
    
}

public abstract class BasePooling<T, TPool>
    where TPool : MonoBehaviour
    where T : Enum
{
    private Dictionary<T, List<TPool>> _poolElements;

    protected BasePooling()
    {
        _poolElements = new Dictionary<T, List<TPool>>();
    }

    public void Reset()
    {
        _poolElements.Clear();
    }

    public TPool GetPoolElement(T type, TPool decorElement, Transform parent = null)
    {
        var state = _poolElements.TryGetValue(type, out var result);

        if (!state)
        {
            result = new List<TPool>();
            _poolElements.Add(type, result);
        }

        for (int i = 0; i < result.Count; i++)
        {
            if (!result[i])
            {
                result.Remove(result[i]);
                continue;
            }
            
            if (!result[i].gameObject.activeInHierarchy)
            {
                result[i].gameObject.SetActive(true);
                return result[i];
            }
        }

        var element = Object.Instantiate(decorElement, parent);
        result.Add(element);
        return element;
    }
}