using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonitorControl : MonoBehaviour
{
    public float maxFOV = 100f;
    public float activationChance = 0.15f;

    private Transform playerTransform;
    private Light tvLight;
    private Image tvScreen;
    private float curFOV;
    private bool isOperating = false;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        tvLight = GetComponent<Light>();
        tvScreen = GetComponentInChildren<Image>();

        StartCoroutine(Operation());
    }

    public void Operate()
    {
        if (isOperating)
        {
            isOperating = false;
            tvLight.enabled = false;
            tvScreen.enabled = false;

            StartCoroutine(Operation());
        }
    }

    private void TriggerOperation()
    {
        if (curFOV > maxFOV * 0.5f && Random.Range(0, 100) < 100 * activationChance)
        {
            isOperating = true;
            tvLight.enabled = true;
            tvScreen.enabled = true;
        }
    }

    private IEnumerator Operation()
    {
        while (!isOperating)
        {
            curFOV = Vector3.Angle(playerTransform.forward, transform.position - playerTransform.position);
            TriggerOperation();

            yield return new WaitForSeconds(1f);
        }
    }
}
