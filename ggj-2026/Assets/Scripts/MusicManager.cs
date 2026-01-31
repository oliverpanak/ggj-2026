using System.Collections;
using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private float[] durations;
    [SerializeField] private StudioEventEmitter eventEmitter;

    private float time; //Contains the remaint from the last music snippet

    private float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartDelayed(1));
    }

    private IEnumerator StartDelayed(float delay)
    {
        eventEmitter.EventInstance.setParameterByName("Sections", 1f);

        //Wait until the intro is over. Because the first section will always be 1f
        yield return Wait(3f);
        //Wait 1 second more
        yield return Wait(delay);

        int index = Random.Range(0, durations.Length);
        StartCoroutine(SetAudio(index));
    }

    private IEnumerator SetAudio(int index)
    {
        var result = eventEmitter.EventInstance.setParameterByName("Sections", (float)(index + 1));

        float time1 = timer;
        yield return Wait(durations[index]);
        float time2 = timer;
        Debug.LogWarning($"{index} took {time2 - time1}");

        index = Random.Range(0, durations.Length);
        StartCoroutine(SetAudio(index));
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
