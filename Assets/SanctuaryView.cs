using UnityEngine;
using Vengadores.InjectionFramework;

public class SanctuaryView : MonoBehaviour
{
    [Inject] private SanctuaryController _sanctuaryController;

    private void OnTriggerEnter(Collider other)
    {
        _sanctuaryController.StartHealing();
    }

    private void OnTriggerExit(Collider other)
    {
        _sanctuaryController.StopHealing();
    }
}