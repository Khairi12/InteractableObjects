using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public enum MonsterMoveState { IDLE, CHASING, SEEKING, PORTING }

public class MonsterMovement : MonoBehaviour
{
    [HideInInspector] public bool phaseEnabled = true;
    [HideInInspector] public bool movementEnabled = true;

    public float defaultSpeed = 3f;         // default speed
    public float chaseSpeed = 6f;           // chase speed
    public float phaseSpeed = 1f;           // phase speed
    public float maxSpeed = 9f;             // max speed value for chasing
    public float speedMultiplier = 0.1f;    // speed multiplier for chasing
    public float minStopDist = 0.1f;        // min stop distance from destination
    public float maxStopDist = 1f;          // max stop distance
    public float minIdle = 0.1f;            // min idle time
    public float maxIdle = 5f;              // max idle time
    public float fovRange = 50f;            // max visible range
    public float fovAngle = 160f;           // max visible FOV

    private MonsterMoveState moveState;     // current state of movement
    private GameObject[] waypoints;         // contains possible destinations
    private GameObject[] phaseableWalls;    // contains all walls that are phaseable
    private NavMeshAgent navagent;          // monster navigation agent
    private Transform playerTransform;      // player transform for player position
    private Vector3 destination;            // current destination
    private Vector3 monstersPosition;       // monsters current position
    private Vector3 monstersLastPosition;   // monsters last position
    private float stepCounter = 3f;         // track distance of last step position
    private float idleCounter = 1f;         // track amount of time idled
    private float outgoingStopDist;         // stop distance after calculations
    private float outgoingMinIdle;          // min idle time after calculations
    private float outgoingMaxIdle;          // max idle time after calculations
    private float outgoingRange;            // visible range after calculations
    private bool insideWall;

    public void SetDestination(Vector3 newDestination)
    {
        destination = newDestination;
        navagent.SetDestination(destination);
    }

    private void Awake()
    {
        GameObject[] minionWPs = GameObject.FindGameObjectsWithTag("MinionWaypoint").ToArray();
        GameObject[] bossWPs = GameObject.FindGameObjectsWithTag("Waypoint").ToArray();

        moveState = MonsterMoveState.IDLE;
        navagent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        phaseableWalls = GameObject.FindGameObjectsWithTag("Phaseable").ToArray();
        waypoints = minionWPs.Concat(bossWPs).ToArray();
        idleCounter = Random.Range(minIdle, maxIdle);

        navagent.speed = defaultSpeed;
        outgoingStopDist = minStopDist;
        outgoingMinIdle = minIdle;
        outgoingMaxIdle = maxIdle;
        outgoingRange = fovRange;
        monstersPosition = transform.position;
        monstersLastPosition = monstersPosition;
    }

    private void Start()
    {
        SetDestination(waypoints[Random.Range(0, waypoints.Length)].GetComponent<Transform>().position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (moveState == MonsterMoveState.CHASING)
            return;

        if (collision.gameObject.tag == "Phaseable")
        {
            insideWall = true;
            navagent.speed = phaseSpeed;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (moveState == MonsterMoveState.CHASING)
            return;

        if (collision.gameObject.tag == "Phaseable")
        {
            insideWall = false;
            navagent.speed = defaultSpeed;
        }
    }

    private void DisableWallPhase(bool setting)
    {
        foreach (GameObject wallObj in phaseableWalls)
        {
            wallObj.GetComponent<NavMeshObstacle>().enabled = setting;

            if (setting)
            {
                wallObj.GetComponent<NavMeshObstacle>().carving = setting;
            }
        }
    }

    private bool PlayerHeard()
    {
        return false;
    }

    private bool PlayerVisible(Vector3 transformPos, Vector3 playerPos, float range)
    {
        RaycastHit hit;
        float curAngle = Vector3.Angle(playerTransform.position - transform.position, transform.forward);

        if (Vector3.Distance(transformPos, playerPos) < range)
        {
            if (Physics.Raycast(transformPos, playerPos - transformPos, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    if (curAngle < fovAngle * 0.5f)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void Stuck()
    {
        stepCounter -= Time.deltaTime;

        if (stepCounter <= 0f)
        {
            monstersPosition = transform.position;

            if (Vector3.Distance(monstersPosition, monstersLastPosition) < 0.05f)
            {
                SetDestination(waypoints[Random.Range(0, waypoints.Length)].GetComponent<Transform>().position);
                moveState = MonsterMoveState.SEEKING;
                navagent.speed = defaultSpeed;
            }
            else
            {
                monstersLastPosition = transform.position;
                stepCounter = 3f;
            }
        }
    }

    private void Idle()
    {
        idleCounter -= Time.deltaTime;

        if (idleCounter <= 0f)
        {
            if (!insideWall)
            {
                DisableWallPhase(Random.Range(0f, 1f) >= 0.5f ? true : false);
            }

            navagent.isStopped = false;
            moveState = MonsterMoveState.SEEKING;
            idleCounter = Random.Range(outgoingMinIdle, outgoingMaxIdle);

        }
        else if (PlayerVisible(transform.position, playerTransform.position, outgoingRange))
        {
            DisableWallPhase(false);
            navagent.isStopped = false;
            navagent.speed = chaseSpeed;
            moveState = MonsterMoveState.CHASING;
            idleCounter = Random.Range(outgoingMinIdle, outgoingMaxIdle);
        }
    }

    private void Seek()
    {
        if (PlayerVisible(transform.position, playerTransform.position, outgoingRange))
        {
            DisableWallPhase(false);
            navagent.speed = chaseSpeed;
            moveState = MonsterMoveState.CHASING;
        }
        else if (Vector3.Distance(transform.position, destination) < outgoingStopDist)
        {
            navagent.isStopped = true;
            moveState = MonsterMoveState.IDLE;
            outgoingStopDist = Random.Range(minStopDist, maxStopDist);
            SetDestination(waypoints[Random.Range(0, waypoints.Length)].GetComponent<Transform>().position);
        }
    }

    private void Chase()
    {
        if (PlayerVisible(transform.position, playerTransform.position, outgoingRange))
        {
            SetDestination(playerTransform.position);

            if (moveState == MonsterMoveState.CHASING && navagent.speed <= maxSpeed)
                navagent.speed += speedMultiplier * Time.deltaTime;
        }
        else if (Vector3.Distance(transform.position, destination) > outgoingRange)
        {
            navagent.speed = defaultSpeed;
            navagent.isStopped = true;
            moveState = MonsterMoveState.IDLE;
        }
    }

    private void Teleport()
    {
        Fade fadeScript = GetComponent<Fade>();

        fadeScript.StartFadeOut();
        // teleport
        // wait until fade in
        // start seeking
    }

    private void Update()
    {
        if (!movementEnabled)
            return;

        if (moveState != MonsterMoveState.IDLE)
            Stuck();

        if (moveState == MonsterMoveState.SEEKING)
            Seek();
        else if (moveState == MonsterMoveState.CHASING)
            Chase();
        else if (moveState == MonsterMoveState.IDLE)
            Idle();
        else if (moveState == MonsterMoveState.PORTING)
            Teleport();
    }
}
