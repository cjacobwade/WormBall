using UnityEngine;
using System.Collections;

public class UVScroll : MonoBehaviour 
{
	[SerializeField] Vector2 scrollSpeed;
	[SerializeField] Vector2 initOffset;

	void Awake()
	{
		initOffset = GetComponent<Renderer>().material.mainTextureOffset;
	}

	void FixedUpdate () 
	{
		GetComponent<Renderer>().material.mainTextureOffset += initOffset + scrollSpeed * Time.deltaTime;
	}
}
