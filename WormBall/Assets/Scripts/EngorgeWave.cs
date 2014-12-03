using UnityEngine;
using System.Collections;

public class EngorgeWave : MonoBehaviour 
{
	[SerializeField] WormText wormText;

	[SerializeField] Color startColor;
	[SerializeField] Color endColor;

	[SerializeField] float startSize;
	[SerializeField] float endSize;

	[SerializeField] AnimationCurve changeCurve;
	[SerializeField] float changeSpeed = 1f;

	void Update () 
	{
		float lerpValue = changeCurve.Evaluate(Mathf.Sin(Time.realtimeSinceStartup * changeSpeed) * 0.5f + 0.5f);

		Color assignColor = Color.Lerp(startColor, endColor, lerpValue);
		wormText.SetColor(assignColor, assignColor);

		wormText.fontSize = (int)Mathf.Lerp(startSize, endSize, lerpValue);
	}
}
