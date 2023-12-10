using System;
using UnityEngine;

public class UpdateController : MonoBehaviour
{
    [SerializeField] private TriggerCheckController _triggerController;

    private void Awake()
    {
        _triggerController.TriggerEnterRegister();
    }
}