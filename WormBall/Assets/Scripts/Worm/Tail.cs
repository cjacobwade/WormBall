using UnityEngine;
using System.Collections;

public class Tail : MonoBehaviour 
{
	[HideInInspector] public Worm worm;
	[SerializeField] float seekRadius = 1.1f;
	[SerializeField] float fleeRadius = 0.8f;
	CircleCollider2D triggerCol;

	void Awake () 
	{
		triggerCol = gameObject.AddComponent<CircleCollider2D>();
		triggerCol.radius = seekRadius;
		triggerCol.isTrigger = true;
	}

	public void Setup(Worm inWorm)
	{
		worm = inWorm;
		triggerCol.radius = worm.carrying ? fleeRadius : seekRadius;
	}
}
