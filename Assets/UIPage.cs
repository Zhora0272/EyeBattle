using UnityEngine;

public class UIPage : MonoBehaviour
{
    [field:SerializeField] public UIPageType PageTye { get; private set; }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
