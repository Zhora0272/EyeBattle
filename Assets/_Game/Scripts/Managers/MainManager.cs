using System.Collections.Generic;
using Smashlab.Pattern;

public class MainManager : Singleton<MainManager>
{
    private static readonly List<object> ManagersList = new();

    public static T GetManager<T>() where T : MonoManager
    {
        foreach (var item in ManagersList)
        {
            if (item.GetType() == typeof(T)) return (T)item;
        }
        return null;
    }

    public static void Register<T>(T t) where T : MonoManager => 
        ManagersList.Add(t);
    public static void UnRegister<T>(T t) where T : MonoManager =>
        ManagersList.Remove(t);
}
