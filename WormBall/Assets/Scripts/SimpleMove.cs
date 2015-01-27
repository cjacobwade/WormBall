using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour 
{
	[SerializeField] Vector3 moveDir;

	void FixedUpdate()
	{
		transform.position += moveDir;
	}
}
