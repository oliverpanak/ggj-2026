using TMPro;
using UnityEngine;

public class DebugRenderer : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
		text.text = $"FPs {1f / Time.fixedDeltaTime}";
    }
}
