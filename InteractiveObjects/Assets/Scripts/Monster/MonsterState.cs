using UnityEngine;

public enum MonsterMoodState { CALM, CAUTIOUS, STRESSED, VIOLENT }

public class MonsterState : MonoBehaviour
{
    [HideInInspector]
    public MonsterMoodState moodState;

    public float mood;

    private bool moodEnabled;

    // PUBLIC METHODS
    public void ToggleMoodRegeneration(bool enabled)
    {
        moodEnabled = enabled ? true : false;
    }

    // PRIVATE METHODS
    private void Awake()
    {
        mood = 100f;
        moodEnabled = true;
        moodState = MonsterMoodState.CALM;
    }

    private void RegenerateMood()
    {
        if (mood <= 0)
        {
            mood += Time.deltaTime;
            UpdateMood();
        }
    }

    private void UpdateMood()
    {
        if (mood >= 75f)
            moodState = MonsterMoodState.CALM;
        else if (mood >= 50f && mood < 75f)
            moodState = MonsterMoodState.CAUTIOUS;
        else if (mood >= 25f && mood < 50f)
            moodState = MonsterMoodState.STRESSED;
        else
            moodState = MonsterMoodState.VIOLENT;
    }

    private void Update()
    {
        if (moodEnabled)
            RegenerateMood();
    }
}