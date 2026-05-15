using UnityEngine;
using UnityEngine.AI;

public class ElaraFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 3.5f;
    public float stoppingDistance = 2f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
        agent.speed = followSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.autoBraking = true;
    }

    void Update()
    {
        if (player == null) return;
        agent.destination = player.position;
    }
}
