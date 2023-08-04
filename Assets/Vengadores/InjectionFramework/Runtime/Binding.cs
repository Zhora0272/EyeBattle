using System.Collections.Generic;
using UnityEngine;

namespace Vengadores.InjectionFramework
{
    /**
     * Helper MonoBehaviour for adding scene components to the DiContainer during install phase.
     * The components are accessible with [Inject] attribute after install phase.
     */
    public class Binding : MonoBehaviour
    {
        public string Id;
        public List<Component> Components;
    }
}
