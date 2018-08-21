using UnityEngine;

public class MonsterEye : MonoBehaviour
{
    public float range;
    private bool eyeEnabled = true;

    // PUBLIC METHODS
    private void ToggleEyeSearch(bool enabled)
    {
        eyeEnabled = enabled ? true : false;
    }

    // PRIVATE METHODS
    private void Awake()
    {
        //playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;
        //outgoingRange = range;
    }

    private void EyeSearch()
    {

    }

    private void Update ()
    {
        if (eyeEnabled)
            EyeSearch();
	}
}
