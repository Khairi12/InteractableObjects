using UnityEngine;

public class Shake : MonoBehaviour
{
    public float movementAmount = 0.005f;
    public float rotationAmount = 1f;
    public float speed = 50f;

    public bool isRotating = false;
    public bool isMoving = false;

    // coroutine (

	private void Update ()
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x + Mathf.Sin(Time.time * Random.Range(0f, speed)) * movementAmount, 
            transform.localPosition.y + Mathf.Sin(Time.time * Random.Range(0f, speed)) * movementAmount, 
            transform.localPosition.z + Mathf.Sin(Time.time * Random.Range(0f, speed)) * movementAmount);

        transform.localRotation = Quaternion.Euler(new Vector3(
            transform.localRotation.x + Mathf.Sin(Time.time * speed) * Random.Range(0f, rotationAmount),
            transform.localRotation.y + Mathf.Sin(Time.time * speed) * Random.Range(0f, rotationAmount),
            transform.localRotation.z + Mathf.Sin(Time.time * speed) * Random.Range(0f, rotationAmount)));
	}
}
