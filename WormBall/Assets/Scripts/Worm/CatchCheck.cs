using UnityEngine;
using System.Collections;

public class CatchCheck : MonoBehaviour 
{
	[HideInInspector] public Worm worm;
	[SerializeField] float despawnTime;

	void Awake () 
	{
		Destroy(gameObject, despawnTime);
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		Ball ball = col.GetComponent<Ball>();
		if(ball && ball.canCatch && !worm.carrying)
		{
			worm.Catch();
			Destroy(col.gameObject);
		}
	}
}
