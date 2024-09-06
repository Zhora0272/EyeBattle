using UnityEngine;
using UnityEngine.AI;

public abstract class BaseNavMeshAgentController : CachedMonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    
    public void SetTargetPosition(Vector3 position)
    {
        _agent.SetDestination(position);
    }
    
    protected void Awake()
    {
        _agent ??= GetComponent<NavMeshAgent>();
    }
}