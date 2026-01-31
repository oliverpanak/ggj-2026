using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;

public class GameManager_Y : MonoBehaviour
{
    public static GameManager_Y Instance { get; private set; }
    
    [SerializeField] private float cameraSpeed;

    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private SplineAnimate splineAnimator;
    [SerializeField] private ConveyerBelt[] initialBelts;

	private Queue<Vector3> stampPositions = new();

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Too many GameManager_Ys in the scene");
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineAnimator.Container = splineContainer;
        splineAnimator.MaxSpeed = cameraSpeed;
        StartCoroutine(DelayedStart());
        foreach (ConveyerBelt belt in initialBelts)
            Add(belt);
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
			if (part == null)
			{
				Debug.LogWarning("Conveyer belt has children that are not conveyer belt parts");
			}
			else
			{
				Add(part);
				if(part.stamp != null)
					stampPositions.Enqueue(part.transform.position);
			}
        }
    }
}
