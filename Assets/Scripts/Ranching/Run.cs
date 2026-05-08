using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private float timer;

    [Header("游走设置")]
    public float walkRadius = 10f;
    public float idleTime = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = idleTime;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 【关键修复】：增加 agent.isOnNavMesh 检查，防止报错
        if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
        {
            // 只有当路径不再计算中，且距离目的地很近时，才重新寻找新位置
            if (timer >= idleTime && !agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetNewRandomDestination();
                timer = 0;
            }
        }
    }

    void SetNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}