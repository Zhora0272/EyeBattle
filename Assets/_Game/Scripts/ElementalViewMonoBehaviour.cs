using UnityEngine;

public class ElementalViewMonoBehaviour<T> : MonoBehaviour
    where T : MonoBehaviour
{
    protected T[] Elements;
    protected UIManager UiManager;

    public virtual void Awake()
    {
        Elements = GetComponentsInChildren<T>(true);
    }

    protected virtual void Start()
    {
        UiManager = MainManager.GetManager<UIManager>();
    }
}