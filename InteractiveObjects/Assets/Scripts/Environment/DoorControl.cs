using UnityEngine;
using System.Collections;

public class DoorControl : MonoBehaviour
{
    public enum Move { LEFT, RIGHT };
    public Move doorMove = Move.LEFT;

    public float doorSpeed = 1f;
    public float doorOpenDist = 1f;
    public float doorInterval = 10f;
    public float doorOpenRange = 5f;
    public float doorFaultRange = 50f;
    public float doorTriggerChance = 0.5f;

    private Transform monster;
    private Vector3 minPos;
    private Vector3 maxPos;
    private Vector3 endPos;
    private Vector3 curPos;
    private float minSpeed = 0.1f;
    private float minInterval = 0.1f;
    private float minStopDist = 0.1f;
    private float maxSpeed;
    private float maxInterval;
    private float maxStopDist;
    private float outDoorSpeed;
    private float outDoorStopDist;
    private float outDoorInterval;
    private float monsterDistance;
    private bool isDoorOpen = false;
    private bool isDoorOperating = false;
    private bool isDoorInteracting = false;

    //--------------------------------------------------------------------------------------------------------------------
    // Public Methods
    //--------------------------------------------------------------------------------------------------------------------

    public void PlayerOpenControl()
    {
        if (!isDoorInteracting)
        {
            isDoorOperating = true;
            isDoorInteracting = true;

            StopCoroutine(isDoorOpen ? AutoCloseDoor() : AutoOpenDoor());
            StartCoroutine(ManualOpenDoor(0.25f));
        }
    }

    //--------------------------------------------------------------------------------------------------------------------
    // Private Methods
    //--------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster").GetComponent<Transform>();

        float maxXLocalPosition = doorMove == Move.LEFT ?
            transform.localPosition.x - doorOpenDist :
            transform.localPosition.x + doorOpenDist;

        minPos = transform.localPosition;
        maxPos = new Vector3(
            maxXLocalPosition,
            transform.localPosition.y,
            transform.localPosition.z);

        maxSpeed = doorSpeed;
        maxInterval = doorInterval;
        maxStopDist = doorOpenDist;

        UpdateStopDist();
        UpdateInterval();
        UpdateSpeed();
    }

    // bug - door position spasm when opening door while curently closing
    private IEnumerator ManualOpenDoor(float speed)
    {
        speed = doorMove == Move.LEFT ? speed : -speed;

        while (Input.GetKey(KeyCode.Q) && 
            doorMove == Move.LEFT && curPos.x > maxPos.x ||
            doorMove == Move.RIGHT && curPos.x < maxPos.x)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x - speed * Time.deltaTime,
                transform.localPosition.y,
                transform.localPosition.z);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        isDoorOperating = false;
        isDoorInteracting = false;
    }

    private IEnumerator AutoOpenDoor()
    {
        while (!isDoorInteracting &&
            doorMove == Move.LEFT && curPos.x > endPos.x ||
            doorMove == Move.RIGHT && curPos.x < endPos.x)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x - outDoorSpeed * Time.deltaTime,
                transform.localPosition.y,
                transform.localPosition.z);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (!isDoorInteracting)
        {
            yield return new WaitForSeconds(outDoorInterval);

            isDoorOpen = true;
            isDoorOperating = false;
            transform.localPosition = endPos;

            //Debug.Log("position set to open");
        }
    }

    private IEnumerator AutoCloseDoor()
    {
        while (!isDoorInteracting &&
            doorMove == Move.LEFT && curPos.x < endPos.x ||
            doorMove == Move.RIGHT && curPos.x > endPos.x)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x + outDoorSpeed * Time.deltaTime,
                transform.localPosition.y,
                transform.localPosition.z);
            
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (!isDoorInteracting)
        {
            yield return new WaitForSeconds(outDoorInterval);

            isDoorOpen = false;
            isDoorOperating = false;
            transform.localPosition = endPos;
            
            //Debug.Log("position set to close");
        }
    }

    private void SetSpeed(float nOutSpeed)
    {
        outDoorSpeed = doorMove == Move.LEFT ? nOutSpeed : -nOutSpeed;
    }

    private void SetInterval(float nOutInterval)
    {
        outDoorInterval = nOutInterval;
    }

    private void SetStopDist(float nRequiredClicks)
    {
        outDoorStopDist = outDoorStopDist - maxStopDist / nRequiredClicks;
        outDoorStopDist = Mathf.Clamp(outDoorStopDist, Mathf.Abs(minPos.x), Mathf.Abs(maxPos.x));
        outDoorStopDist = doorMove == Move.LEFT ? outDoorStopDist : -outDoorStopDist;

        endPos = new Vector3(
            isDoorOpen ? curPos.x + outDoorStopDist : curPos.x - outDoorStopDist,
            transform.localPosition.y,
            transform.localPosition.z);
    }

    private void UpdateSpeed()
    {
        float distance = Vector3.Distance(monster.position, transform.position);

        outDoorSpeed = doorSpeed / distance;
        outDoorSpeed = Mathf.Clamp(outDoorSpeed, minSpeed, maxSpeed);
        outDoorSpeed = doorMove == Move.LEFT ? outDoorSpeed : -outDoorSpeed;
    }

    private void UpdateInterval()
    {
        float distance = Vector3.Distance(monster.position, transform.position);
        float maxClamp = (distance / doorFaultRange) * maxInterval;
        float minClamp = minInterval;

        outDoorInterval = Mathf.Clamp(distance, minClamp, maxClamp);
    }
    
    private void UpdateStopDist()
    {
        outDoorStopDist = isDoorOpen ?
            Random.Range(minStopDist, Mathf.Abs(curPos.x - minPos.x)) :
            Random.Range(minStopDist, Mathf.Abs(curPos.x - maxPos.x));
        
        outDoorStopDist = doorMove == Move.LEFT ? outDoorStopDist : -outDoorStopDist;

        endPos = new Vector3(
            isDoorOpen? curPos.x + outDoorStopDist : curPos.x - outDoorStopDist,
            transform.localPosition.y,
            transform.localPosition.z);
    }

    private void TriggerDoorControl()
    {
        if (!isDoorOperating && !isDoorInteracting && monsterDistance < doorOpenRange + doorFaultRange && 
            Random.Range(0f, monsterDistance) < (doorOpenRange + doorFaultRange) * doorTriggerChance)
        {
            isDoorOperating = true;

            UpdateSpeed();
            UpdateInterval();
            UpdateStopDist();

            StartCoroutine(isDoorOpen ? AutoCloseDoor() : AutoOpenDoor());
        }
    }

    private void DisableDoor()
    {

    }

    private void Update()
    {
        curPos = transform.localPosition;
        monsterDistance = Vector3.Distance(monster.position, transform.position);

        if (monsterDistance < doorOpenRange)
            DisableDoor();
        else if (monsterDistance < doorOpenRange + doorFaultRange)
            TriggerDoorControl();
    }
}
 