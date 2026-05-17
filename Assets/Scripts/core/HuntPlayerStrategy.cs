using UnityEngine;
using UnityEngine.AI;

public class HuntPlayerStrategy : IMovementStrategy
{
    public void ExecuteMove(NavMeshAgent agent, Transform selfTransform)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.transform.position);
        }
    }
}