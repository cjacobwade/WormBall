using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour 
{
	public void SelfDestroy(float wait)
	{
		StartCoroutine(DoSelfDestroy(wait));
	}

	IEnumerator DoSelfDestroy(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}
