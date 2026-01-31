using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float[] durations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TestAudio(0));
    }

    private IEnumerator TestAudio(int index)
    {
        float time = 0;

        Debug.Log($"Playing {clips[index].name} with duration {durations[index]}");
        source.PlayOneShot(clips[index]);

        while(time < durations[index])
        {
            time += Time.deltaTime;
            yield return null;
        }

        index = Random.Range(0, durations.Length);
        StartCoroutine(TestAudio(index));
    }
}
