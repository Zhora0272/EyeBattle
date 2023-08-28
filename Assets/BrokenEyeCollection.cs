using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEyeCollection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("BrokenEye"))
        {
            other.transform.DOMove(other.transform.position + Vector3.up, 0.5f).onComplete = () =>
            {
                other.transform.DOMove(transform.position, 0.5f);
                other.transform.DOScale(0, 0.7f).OnComplete(() =>
                {
                    other.gameObject.SetActive(false);
                });
            };
        }
    }
}
