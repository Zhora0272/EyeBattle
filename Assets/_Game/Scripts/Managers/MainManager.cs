using System;
using System.Collections.Generic;
using Smashlab.Pattern;

public class MainManager : Singleton<MainManager>
{
    private static readonly Dictionary<Type, List<Action<MonoManager>>> WaitManagerList = new();
    private static readonly List<object> ManagersList = new();

    /// <summary>
    /// !!!call this function only after awake!!!
    /// this function is init in awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetManager<T>() where T : MonoManager
    {
        foreach (var item in ManagersList)
        {
            if (item.GetType() == typeof(T)) return (T)item;
        }

        return null;
    }

    public static void WaitManager<T>(Action<MonoManager> manager) where T : MonoManager
    {
        foreach (var item in ManagersList)
        {
            if (item.GetType() == typeof(T))
            {
                manager.Invoke((T)item);
            }
        }
        
        if (WaitManagerList.TryGetValue(typeof(T), out var result))
        {
            result.Add(manager);
        }
        else
        {
            var list = new List<Action<MonoManager>> { manager };
            WaitManagerList.Add(typeof(T), list);
        }
    }

    public static void Register<T>(T t) where T : MonoManager
    {
        ManagersList.Add(t);
        
        if (WaitManagerList.TryGetValue(t.GetType(), out var result))
        {
            foreach (var item in result)
            {
                item.Invoke(t);
            }
            WaitManagerList.Remove(t.GetType());
        }
    }

    public static void UnRegister<T>(T t) where T : MonoManager =>
        ManagersList.Remove(t);
}