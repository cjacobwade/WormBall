using UnityEngine;
using System.Collections;

public class UVScroll : MonoBehaviour 
{
	[SerializeField] Vector2 scrollSpeed;
	[SerializeField] Vector2 initOffset;

	void Awake()
	{
		initOffset = renderer.material.mainTextureOffset;
	}

	void FixedUpdate () 
	{
		renderer.material.mainTextureOffset += initOffset + scrollSpeed * Time.deltaTime;
	}
}
