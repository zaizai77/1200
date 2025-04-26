using UnityEngine;

public class NavMeshMovement : MonoBehaviour, IMovement
{
    private UnityEngine.AI.NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void Move(Vector3 dir)
    {
        Vector3 dest = transform.position + dir.normalized * agent.speed;
        agent.SetDestination(dest);
    }
}
