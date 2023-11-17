using UnityEngine;

namespace _Game.Scripts.UI
{
    public class UISubPage : UIPageBase
    {
        [field: SerializeField] public UISubPageType PageTye { get; private set; }
    }
}