using System.Collections;
using UnityEngine;

public class RotateInteraction : MonoBehaviour
{
    public float rotateSpeed = 1f;
    public float rotateAmount = 90f;
    public bool rotateClockwise = true;

    private Quaternion curRot;
    private Quaternion startRot;
    private Quaternion endRot;
    private bool isOpen = false;
    private bool isOperating = false;

    private void Start()
    {
        startRot = transform.localRotation;
        endRot = Quaternion.Euler(new Vector3(
            startRot.x, 
            startRot.y + rotateAmount, 
            startRot.z));
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
        while (curRot.eulerAngles.y < endRot.eulerAngles.y)
        {
            transform.localRotation = Quaternion.RotateTowards(curRot, endRot, rotateSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        isOpen = true;
        isOperating = false;
        yield return null;
    }

    private IEnumerator Close()
    {
        while (curRot.eulerAngles.y > startRot.eulerAngles.y)
        {
            transform.localRotation = Quaternion.RotateTowards(curRot, startRot, rotateSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        isOpen = false;
        isOperating = false;
        yield return null;
    }

    private void Update()
    {
        curRot = transform.localRotation;
    }
}
