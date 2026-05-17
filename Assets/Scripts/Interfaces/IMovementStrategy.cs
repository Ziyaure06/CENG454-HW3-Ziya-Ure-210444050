using UnityEngine;
using UnityEngine.AI;

public interface IMovementStrategy
{
    // NavMeshAgent'ý hedefe yönlendirecek olan ana metot
    void ExecuteMove(NavMeshAgent agent, Transform selfTransform);
}