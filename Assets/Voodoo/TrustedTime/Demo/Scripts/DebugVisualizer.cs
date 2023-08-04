using TMPro;
using UnityEngine;

namespace Voodoo.Utils
{
    public class DebugVisualizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeDisplay;
        private void Update()
        {
            _timeDisplay.text = TrustedTime.ToString(TrustedTime.Timestamp,@"yyyy-MM-dd HH:mm:ss");
        }
    }
} 