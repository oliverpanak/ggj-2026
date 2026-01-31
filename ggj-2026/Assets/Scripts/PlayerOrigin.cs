using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerOrigin : MonoBehaviour
{
    [SerializeField] SplineAnimate animator;

    private void Start()
    {
        StartCoroutine(StartMovement());
    }

    private IEnumerator StartMovement()
    {
        yield return null;
        animator.enabled = true;
        animator.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
