using UnityEngine;

namespace _Game.Scripts.UI
{
    public class UIPage : UIPageBase
    {
        [field: SerializeField] public UIPageType PageTye { get; private set; }
    }
}