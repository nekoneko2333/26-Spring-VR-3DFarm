using UnityEngine;
using UnityEngine.AI; // 必须引用寻路命名空间

public class AnimalAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private float timer;

    [Header("游走设置")]
    public float walkRadius = 10f; // 随机走动的范围
    public float idleTime = 5f;    // 停顿时间

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = idleTime;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 如果停留时间到了，且奶牛现在没在走路（到达了目的地）
        if (timer >= idleTime && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNewRandomDestination();
            timer = 0;
        }
    }

    void SetNewRandomDestination()
    {
        // 在球体内找随机点
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // 关键：将随机点映射到刚才生成的“蓝色区域”上
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}