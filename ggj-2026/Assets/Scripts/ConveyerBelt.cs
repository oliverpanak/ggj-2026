using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public class ConveyerBelt : MonoBehaviour
{
	[SerializeField] private bool alreadyExists = false;
	void Start()
	{
		if(!alreadyExists)
			GameManager_Y.Instance.Add(this);
	}
}
