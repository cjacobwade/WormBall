using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour 
{
	float size = 1000.0f; // how many grid lines drawn in each direction
	float drawDistance = 1000000.0f; // how far out are the lines drawn

	public float width = 1.0f;
	public float height = 1.0f;
	
	void Start () 
	{
	}
	
	void Update () 
	{
	}
	
	void OnDrawGizmos()
	{
		Vector3 pos = Camera.current.transform.position;
		
		for (float y = pos.y - size; y < pos.y + size; y += height)
		{
			Gizmos.DrawLine(transform.rotation * new Vector3(-drawDistance, Mathf.Floor(y/height) * height, 0.0f),
			                transform.rotation * new Vector3(drawDistance, Mathf.Floor(y/height) * height, 0.0f));
		}
		
		for (float x = pos.x - size; x < pos.x + size; x+= width)
		{
			Gizmos.DrawLine(transform.rotation * new Vector3(Mathf.Floor(x/width) * width, -drawDistance, 0.0f),
			                transform.rotation * new Vector3(Mathf.Floor(x/width) * width, drawDistance, 0.0f));
		}
	}
}