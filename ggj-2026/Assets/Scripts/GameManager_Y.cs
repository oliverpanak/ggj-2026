using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager_Y : MonoBehaviour
{
    private class StampTime
    {
        public double cameraStop, stamp, cameraGo;
        public int progress;
    }
    public static GameManager_Y Instance { get; private set; }

    [SerializeField] private float cameraSpeed;

    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private SplineAnimate splineAnimator;
    [SerializeField] private ConveyerBelt[] initialBelts;
    [SerializeField, Tooltip("Please put them in order xox")] private GameObject[] sections;

    private Queue<StampTime> stampTimes = new();
    private double timer = 0;
    private double nextTimeStep;
    private static double s_conveyerTime = 1.5;
    private static double s_stampTime = 3;
    private double nextSpawnPosition;
    private static double s_conveyerSize = 2;
    public event Action onCameraStop;
    public event Action onStamp;
    public event Action onCameraGo;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Too many GameManager_Ys in the scene");
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 60;

        splineAnimator.Container = splineContainer;
        splineAnimator.MaxSpeed = cameraSpeed;
        StartCoroutine(DelayedStart());
        foreach (ConveyerBelt belt in initialBelts)
            Add(belt);

        onCameraStop += () => { splineAnimator.Pause(); };
        onStamp += () => {};
        onCameraGo += () => { splineAnimator.Play(); };
        MusicManager.Instance.onDecideNextSection += SpawnNextSection;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (stampTimes.Count == 0) return;
        StampTime stamp = stampTimes.Peek();
        if (stamp.cameraStop < timer && stamp.progress == 0)
        {
            onCameraStop?.Invoke();
            stamp.progress++;
        }
        else if (stamp.stamp < timer && stamp.progress == 1)
        {
            onStamp?.Invoke();
            stamp.progress++;
        }
        else if (stamp.cameraGo < timer && stamp.progress == 2)
        {
            onCameraGo?.Invoke();
            stampTimes.Dequeue();
        }
    }

    private IEnumerator DelayedStart()
    {
        yield return null;
        splineAnimator.enabled = true;
        splineAnimator.Play();
    }

    private void Add(CBPart part)
    {
        splineContainer.Spline.Add(part.transform.position - transform.position);
    }

    public void Add(ConveyerBelt belt)
    {
        foreach (Transform child in belt.transform)
        {
            CBPart part = child.GetComponent<CBPart>();
            if (part != null)
            {
                Add(part);
                if (part.stamp != null)
                {
                    AddToQueue(belt.type == "C" ? nextTimeStep + 0.75 : nextTimeStep + 1.5, belt.type == "C");
                }

                nextTimeStep += s_conveyerTime;
                nextSpawnPosition += s_conveyerSize;
                if (part.stamp != null) nextTimeStep += belt.type == "C" ? s_stampTime * 0.5 : s_stampTime;
            }
        }
    }

    private void AddToQueue(double stampTime, bool halfTime = false)
    {
        StampTime time = new();
        //Hier weitermachen
        if (halfTime)
        {
            time.cameraStop = stampTime - 0.25 * s_stampTime;
            time.stamp = stampTime;
            time.cameraGo = stampTime + 0.25 * s_stampTime;
        }
        else
        {
            time.cameraStop = stampTime - 0.5 * s_stampTime;
            time.stamp = stampTime;
            time.cameraGo = stampTime + 0.5 * s_stampTime;
        }
        stampTimes.Enqueue(time);
    }

    private void SpawnNextSection(int sectionIndex)
    {
        GameObject prefab = sections[sectionIndex];
        Vector3 position = new Vector3((float)nextSpawnPosition, 0, 0);
        Instantiate(prefab, position, Quaternion.identity);
    }
}
