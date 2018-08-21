using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour
{
    public float lightSpeed;
    public float minLighting;
    public float maxLighting;

    public float disableRange;              // distance required to disable light

    public float flickerRange;              // distance required to enable flickering
    public float flickerChance;             // 0.01 = 1%, 0.99 = 99% activation chance per frame
    public float minFlickerDelay;           // min flicker duration
    public float maxFlickerDelay;           // max flicker duration
    
    public float oscillationRange;          // distance required to enable oscillation
    public float oscillationChance;         // 0.01 = 1%, 0.99 = 99% activation chance per frame

    private Transform monster;
    private new Light light;
    private float monsterDistance;
    private bool isOscillating;
    private bool isFlickering;
    private bool isDisabled;

	private void Start ()
    {
        light = GetComponent<Light>();
        monster = GameObject.FindGameObjectWithTag("Monster").GetComponent<Transform>();

        isOscillating = false;
        isFlickering = false;
        isDisabled = false;
    }

    private IEnumerator Oscillation()
    {
        float oscillationLight = maxLighting;

        while (oscillationLight > minLighting)
        {
            oscillationLight -= lightSpeed * Time.deltaTime;
            light.intensity -= lightSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        while (oscillationLight < maxLighting)
        {
            oscillationLight += lightSpeed * Time.deltaTime;
            light.intensity += lightSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // pause after oscillation
        // closer ghost = shorter pause, farther = longer pause.
        yield return new WaitForSeconds(5f);
        
        light.intensity = maxLighting;
        isOscillating = false;
    }
    
    private IEnumerator Flickering()
    {
        // closer ghost = more flickers, farther = less flickers
        int flickerAmount = Random.Range(0, 20);

        while (flickerAmount > 0)
        {
            light.intensity /= 3;
            yield return new WaitForSeconds(0.1f);
            light.intensity *= 3;

            flickerAmount -= 1;
            yield return new WaitForSeconds(Random.Range(minFlickerDelay, maxFlickerDelay));
        }

        // pause after flickering
        // closer ghost = shorter pause, farther = longer pause.
        yield return new WaitForSeconds(5f);

        // reset
        isFlickering = false;
    }

    private void TriggerFlickering()
    {
        if (!isFlickering && monsterDistance < disableRange + flickerRange &&
            Random.Range(0f, monsterDistance) < (flickerRange + disableRange) * flickerChance)
        {
            isFlickering = true;
            StartCoroutine("Flickering");
        }
    }

    private void TriggerOscillation()
    {
        if (!isOscillating && monsterDistance < disableRange + oscillationRange &&
            Random.Range(0f, monsterDistance) < (oscillationRange + disableRange) * oscillationChance)
        {
            isOscillating = true;
            StartCoroutine("Oscillation");
        }
    }

    private void DisableLight()
    {
        if (!isDisabled)
        {

        }
    }
    
    private void Update()
    {
        monsterDistance = Vector3.Distance(monster.position, transform.position);

        if (monsterDistance < disableRange)
            DisableLight();
        else if (monsterDistance < disableRange + flickerRange)
            TriggerFlickering();
        else if (monsterDistance < disableRange + oscillationRange)
            TriggerOscillation();
    }
}
