using UnityEngine;
using System.Collections;

public class AudioUtils : MonoBehaviour 
{
	public void FadeVolume(float startVolume, float endVolume, float time)
	{
		StartCoroutine(DoFadeVolume(startVolume, endVolume, time));
	}

	IEnumerator DoFadeVolume(float startVolume, float endVolume, float time)
	{
		float timer = 0.0f;
		while(timer < time)
		{
			audio.volume = Mathf.Lerp(startVolume, endVolume, timer/time);
			timer += Time.deltaTime;
			yield return 0;
		}
	}
}
