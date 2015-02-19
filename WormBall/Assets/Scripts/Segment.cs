using UnityEngine;
using System.Collections;

public class Segment : MonoBehaviour 
{
	public Worm worm;
	SetupBounds sb;
	Collider2D col;

	new Transform transform;

	void Awake () 
	{
		transform = GetComponent<Transform>();
		col = GetComponent<Collider2D>();
		sb = SetupBounds.instance;
	}

	void Update () 
	{
		if(sb.IsInBounds(transform.position))
		{
			col.enabled = true;
		}
		else
		{
			col.enabled = false;
		}
	}
}