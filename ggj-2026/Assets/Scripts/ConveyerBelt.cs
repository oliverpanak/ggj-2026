using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public class ConveyerBelt : MonoBehaviour
{
	[SerializeField] private float selfdestructTimer = 120;

	[SerializeField] private bool alreadyExists = false;
	void Start()
	{
		if(!alreadyExists)
			GameManager_Y.Instance.Add(this);
	}

	private IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(selfdestructTimer);
		Destroy(gameObject);
	}
}
