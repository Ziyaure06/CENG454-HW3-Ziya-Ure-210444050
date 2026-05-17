using UnityEngine;
using UnityEngine.AI;

public class TargetBaseStrategy : IMovementStrategy
{
    public void ExecuteMove(NavMeshAgent agent, Transform selfTransform)
    {
        // Sahnedeki "Core" veya "Wall" etiketli objeleri hedefler.
        // Performans optimizasyonu için normalde bunlar bir Listede tutulur, 
        // ancak primitives ve tag zorunluluðu için bu þekilde buluyoruz.
        GameObject core = GameObject.FindGameObjectWithTag("Core");

        if (core != null && agent.isOnNavMesh)
        {
            agent.SetDestination(core.transform.position);
        }
    }
}