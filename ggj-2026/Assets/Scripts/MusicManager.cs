using System.Collections;
using UnityEngine;
using FMODUnity;
using System;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    [SerializeField] private float[] durations;
    [SerializeField] private StudioEventEmitter eventEmitter;

    private float time; //Contains the remaint from the last music snippet

    private float timer = 0f;

    //Input is the section that has been chosen. 0 == A, 1 == B, etc.
    public event Action<int> onDecideNextSection;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Too many MusicManagers in the scene");
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartDelayed(0.2f));
    }

    private IEnumerator StartDelayed(float delay)
    {
        eventEmitter.EventInstance.setParameterByName("Sections", 1f);

        //Wait until the intro is over. Because the first section will always be 1f
        yield return Wait(3f);
        //Wait for the delay
        yield return Wait(delay);

        int index = UnityEngine.Random.Range(0, durations.Length);
        StartCoroutine(SetAudio(index, 13.5f));
    }

    private IEnumerator SetAudio(int index, float delay)
    {
        var result = eventEmitter.EventInstance.setParameterByName("Sections", (index + 1));
        onDecideNextSection?.Invoke(index);
        Debug.Log($"Spawned next at {timer} s");

        float time1 = timer;
        yield return Wait(delay);
        float time2 = timer;
        Debug.LogWarning($"{index} took {time2 - time1}");

        float nextDelay = durations[index];
        index = UnityEngine.Random.Range(0, durations.Length);
        StartCoroutine(SetAudio(index, nextDelay));
    }

    private IEnumerator Wait(float time)
    {
        while(this.time < time)
        {
            this.time += Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        this.time -= time;
    }
}
