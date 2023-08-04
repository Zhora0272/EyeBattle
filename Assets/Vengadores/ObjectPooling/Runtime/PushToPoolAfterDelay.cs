using System.Collections;
using UnityEngine;

namespace Vengadores.ObjectPooling
{
    public class PushToPoolAfterDelay : MonoBehaviour
    {
        public float DespawnAfter = 1f;

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(WaitAndDespawn());
        }

        private IEnumerator WaitAndDespawn()
        {
            yield return new WaitForSeconds(DespawnAfter);
            Despawn();
        }

        private void Despawn()
        {
            GameObjectPool.GetPoolOfInstance(gameObject).Push(gameObject);
        }
    }
}