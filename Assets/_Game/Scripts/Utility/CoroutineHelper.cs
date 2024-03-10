using System.Collections;
using UnityEngine;
using System;

namespace _Game.Scripts.Utility
{
    public static class CoroutineHelper
    {
        public static IEnumerator WaitToObjectInit(MonoBehaviour obj, Action actionAfterInit)
        {
            yield return new WaitUntil(() => obj != null);
            actionAfterInit?.Invoke();
        }

        private static void CoroutineDebug(string debug)
        {
            Debug.Log("CoroutineHelper:> " + debug);
        }
    }
}