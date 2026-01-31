using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class GameManager_Y : MonoBehaviour
{
    
    [SerializeField] private float cameraSpeed;

    [SerializeField] private SplineAnimate splineAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineAnimator.MaxSpeed = cameraSpeed;
        StartCoroutine(DelayedStart());
    }


    private IEnumerator DelayedStart()
    {
        yield return null;
        splineAnimator.enabled = true;
        splineAnimator.Play();
    }
}
