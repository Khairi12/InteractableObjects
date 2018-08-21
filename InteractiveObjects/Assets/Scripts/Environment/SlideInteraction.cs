using System.Collections;
using UnityEngine;

public class SlideInteraction : MonoBehaviour
{
    public float openSpeed = 0.5f;
    public float openDistance = 0.5f;

    private Vector3 curPos;
    private Vector3 startPos;
    private Vector3 endPos;

    private bool isOpen = false;
    private bool isOperating = false;

    void Start()
    {
        curPos = transform.localPosition;
        startPos = transform.localPosition;

        endPos = new Vector3(
            openDistance,
            transform.localPosition.y,
            transform.localPosition.z);
    }

    public void Operate()
    {
        if (!isOperating)
        {
            isOperating = true;
            StartCoroutine(isOpen ? Close() : Open());
        }
    }

    private IEnumerator Open()
    {
        while (curPos.x <= endPos.x)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x + openSpeed * Time.deltaTime,
                transform.localPosition.y,
                transform.localPosition.z);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        isOpen = true;
        isOperating = false;
        transform.localPosition = endPos;
        yield return null;
    }

    private IEnumerator Close()
    {
        while (curPos.x >= startPos.x)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x - openSpeed * Time.deltaTime,
                transform.localPosition.y,
                transform.localPosition.z);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        isOpen = false;
        isOperating = false;
        transform.localPosition = startPos;
        yield return null;
    }

    private void Update()
    {
        curPos = transform.localPosition;
    }
}
