using UnityEngine;
using System.Collections;

public class Segment : MonoBehaviour 
{
	SetupBounds sb;
	Collider2D[] cols;

	void Awake () 
	{
		sb = SetupBounds.singleton.instance;
		cols = GetComponents<Collider2D>();
	}

	void Update () 
	{
		if(sb.IsInBounds(transform.position))
		{
			foreach(Collider2D col in cols)
			{
				col.enabled = true;
			}
		}
		else
		{
			foreach(Collider2D col in cols)
			{
				col.enabled = false;
			}
		}
	}
}