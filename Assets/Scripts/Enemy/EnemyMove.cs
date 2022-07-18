using UnityEngine;
using UnityEngine.AI;
using System.Collections;
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("追いかける対象")]
    private Transform target;
    
    private NavMeshAgent myAgent;
    public bool IsPlayerFind = false;
    [SerializeField]
    private bool IsPlayerVisible = false;
    private float run_speed = 1.85f;
    private float walk_speed = 1.0f;
    private RaycastHit hit;
    private Rigidbody rigid_body;
    private GameObject random_spawn_empty;
    private int max_random_x = 2,min_random_x = -20;
    private int max_random_z = 12,min_random_z= 2;
    private WaitForSeconds time = new WaitForSeconds(1);
    [SerializeField]
    private float TrackingTime = 8;
    private float timer = 0;
    private AudioSource audio;
    private Coroutine coroutine;
    void Start()
    {
        // Nav Mesh Agent を取得します。
        myAgent = GetComponent<NavMeshAgent>();

               NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        coroutine = StartCoroutine(RandomTarget());
        audio = SeManager.Instance.Play(transform,SeManager.VOICE,true);
        audio.minDistance = 1f;
        audio.maxDistance = 12.5f;
        audio.volume = 0.05f;
        audio.reverbZoneMix = 1.1f;
        audio.spatialBlend = 1;
        audio.dopplerLevel = 1;
        audio.spread = 0;
        audio.rolloffMode = AudioRolloffMode.Linear;

    }
    private bool IsMagnitude(NavMeshPath path){
        //NavMeshPath path;
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        var length = path.corners[path.corners.Length - 1] - target.position;
        return length.magnitude > 1.0f;
    }
    private bool IsTargetComplate(NavMeshPath path){
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        return path.corners.Length == 0;
    }
    private IEnumerator RandomTarget(){
        random_spawn_empty = new GameObject();
        random_spawn_empty.AddComponent<BoxCollider2D>();
        float x = Random.Range(min_random_x,max_random_x);
        float z = Random.Range(min_random_z,max_random_z);
        random_spawn_empty.transform.position = new Vector3(x,0,z);
        var path = new NavMeshPath();
        int count = 0;
        while(true){
            count++;
            target = random_spawn_empty.transform;
            yield return time;
            if(count == 10 || IsPlayerFind || IsTargetComplate(path) || IsMagnitude(path)){
                Destroy(random_spawn_empty);
                StopCoroutine(coroutine);
                if(!IsPlayerFind) coroutine = StartCoroutine(RandomTarget());
                break;
            }
        }
    }
    void Update(){
        Ray();
        if(IsPlayerFind){
            target = GameController.Instance.GetFpsController.gameObject.transform;
        }
        if(timer <= TrackingTime && !IsPlayerVisible){
            timer += Time.deltaTime;
        }else{
            timer = 0;
        }
        if(IsPlayerFind && timer >= TrackingTime){
            IsPlayerFind = false;
            other = null;
            StartCoroutine(RandomTarget());
        }
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
            transform.position += transform.forward * Time.deltaTime * (!IsPlayerFind ? walk_speed :run_speed);
            // もしもの場合の補正
            //if (Vector3.Distance(nextPoint, transform.position) < 0.5f) transform.position = nextPoint;
        }

        // targetに向かって移動します。
        myAgent.SetDestination(target.position);
        myAgent.nextPosition = transform.position;

    }
    private GameObject other;
    private void OnTriggerStay(Collider other){
        if (other.CompareTag("Player")){
            this.other = other.gameObject;
        }
    }
    private void Ray(){
        if(other != null){
            var diff = other.transform.position - transform.position;
            var distance = 10;
            var direction = diff.normalized;
            Ray ray = new Ray();
            ray.origin = transform.position;
            ray.direction = direction;
            if(Physics.Raycast(ray, out hit, distance)){
                if(hit.transform.gameObject.tag == "Player"){
                    IsPlayerFind = true;
                    IsPlayerVisible = true;
                }else{
                    IsPlayerVisible = false;
                }
            }
        }
    }
}