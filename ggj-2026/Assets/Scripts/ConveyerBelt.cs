using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public class ConveyerBelt : MonoBehaviour
{
	public static ConveyerBelt Instance {get; private set;}
	
	[SerializeField] SplineContainer splineContainer;

	void Awake()
	{
		if (Instance != null)
			Debug.LogError("Too many ConveyerBelts in the scene");
		Instance = this;
	}
	
	void Start()
	{
		InitialiseSpline();
	}
	
	public void Add(CBPart part)
	{
		splineContainer.Spline.Add(part.transform.position);
	}

	private void InitialiseSpline()
	{
		foreach(Transform child in transform)
		{
			CBPart part = child.GetComponent<CBPart>();
			if (part == null)
				Debug.LogWarning("Conveyer belt has children that are not conveyer belt parts");
			Add(part);
		}
    }
}
