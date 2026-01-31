using System.Collections;
using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private float[] durations;
    [SerializeField] private StudioEventEmitter eventEmitter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var result = eventEmitter.EventInstance.setParameterByName("Sections", 1f);
        StartCoroutine(StartDelayed(1));
    }

    private IEnumerator StartDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        int index = Random.Range(0, durations.Length);
        StartCoroutine(SetAudio(index));
    }

    private IEnumerator SetAudio(int index)
    {
        float time = 0;

        var result = eventEmitter.EventInstance.setParameterByName("Sections", (float)(index + 1));
        Debug.Log($"setting track to index {index}");

        while (time < durations[index])
        {
            time += Time.deltaTime;
            yield return null;
        }

        index = Random.Range(0, durations.Length);
        StartCoroutine(SetAudio(index));
    }
}
