using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("追いかける対象")]
    private Transform target;
    
    private NavMeshAgent myAgent;
    public bool IsPlayerFind{get;set;}
    private float run_speed = 3.5f;
    private float walk_speed = 2f;

    void Start()
    {
        // Nav Mesh Agent を取得します。
        myAgent = GetComponent<NavMeshAgent>();

               NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        //NavMeshPath path;
        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        var length = path.corners[path.corners.Length - 1] - target.position;
        if (length.magnitude > 1.0f)
            Debug.Log( "到達しません");
    }

    void Update()
    {

        // 次に目指すべき位置を取得
        var nextPoint = myAgent.steeringTarget;
        Vector3 targetDir = nextPoint - transform.position;

        // その方向に向けて旋回する(120度/秒)
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 120f * Time.deltaTime);

        // 自分の向きと次の位置の角度差が30度以上の場合、その場で旋回
        float angle = Vector3.Angle(targetDir, transform.forward);
        if (angle < 30f)
        {
            transform.position += transform.forward * 1.0f * Time.deltaTime;
            // もしもの場合の補正
            //if (Vector3.Distance(nextPoint, transform.position) < 0.5f) transform.position = nextPoint;
        }

        // targetに向かって移動します。
        myAgent.SetDestination(target.position);
        myAgent.nextPosition = transform.position;

    }
}