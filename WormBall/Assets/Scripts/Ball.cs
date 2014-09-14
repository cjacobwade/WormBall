using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{
	[SerializeField] float maxDistance = 30.0f;
	[SerializeField] float scale = 1.0f;
	[SerializeField] float speedLoss;

	[SerializeField] float scaleSpeed = 1.0f;
	[SerializeField] float scaleTime = 1.5f;
	float scaleTimer = 0.0f;

	public bool canCatch = true;
	Vector3 lastVel;

	public bool spawnBall = true;

	void Awake()
	{

	}

	void Update()
	{ 
		if(IsOutOfBounds())
		{

		}

		lastVel = rigidbody2D.velocity;
	}

	bool IsOutOfBounds()
	{
		SetupBounds sb = SetupBounds.instance;

		if(transform.position.x > sb.worldMax.x || 
		   transform.position.y > sb.worldMax.y || 
		   transform.position.x < sb.worldMin.x || 
		   transform.position.y < sb.worldMin.y)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

//	void CheckOutOfBounds()
//	{
//		SetupBounds sb = SetupBounds.singleton.instance;
//		Vector3 spawnPos = Vector3.zero;
//		bool screenWrap = false;
//
//		if(transform.position.x > sb.worldMax.x)
//		{
//			spawnPos = transform.position;
//			spawnPos.x = -sb.worldMax.x;
//			screenWrap = true;
//		}
//
//		if(transform.position.y > sb.worldMax.y)
//		{
//			spawnPos = transform.position;
//			spawnPos.y = -sb.worldMax.y;
//			screenWrap = true;
//		}
//
//		if(transform.position.x < sb.worldMin.x)
//		{
//			spawnPos = transform.position;
//			spawnPos.x = sb.worldMax.x;
//			screenWrap = true;
//		}
//
//		if(transform.position.y < sb.worldMin.y)
//		{
//			spawnPos = transform.position;
//			spawnPos.y = sb.worldMax.y;
//			screenWrap = true;
//		}
//
//		if(screenWrap && spawnBall)
//		{
//			GameObject newBallObj = WadeUtils.Instantiate(gameObject, spawnPos, transform.rotation);
//			newBallObj.rigidbody2D.velocity = rigidbody2D.velocity;
//			newBallObj.rigidbody2D.angularVelocity = rigidbody2D.angularVelocity;
//
//			Ball newBall = newBallObj.GetComponent<Ball>();
//
//			if(!canCatch)
//			{
//				newBall.StartCoroutine(newBall.ScaleUp(scaleTime - scaleTimer));
//			}
//
//			spawnBall = false;
//			Destroy(collider);
//			Destroy(gameObject, .5f);
//		}
//	}

	public IEnumerator ScaleUp(float currentTimer = 0.0f)
	{
		canCatch = false;
		transform.localScale = Vector3.one * 0.1f;
		scaleTimer = currentTimer;

		while(scaleTimer < scaleTime)
		{
			if(scaleTimer > scaleTime/3.0f)
			{
				gameObject.layer = LayerMask.NameToLayer("Ball");
			}

			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * scale, scaleTimer/scaleTime);
			scaleTimer += Time.deltaTime;
			yield return 0;
		}

		transform.localScale = Vector3.one * scale;
		scaleTimer = 0.0f;
		canCatch = true;
	}

	public IEnumerator TimedDisableCollision(float waitTime)
	{
		Collider2D[] cols = GetComponentsInChildren<Collider2D>();

		foreach(Collider2D col in cols)
		{
			col.enabled = false;
		}

		yield return new WaitForSeconds(waitTime);

		foreach(Collider2D col in cols)
		{
			col.enabled = true;
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		Vector2 velocity = lastVel;
		rigidbody2D.velocity = Vector3.Reflect(velocity, col.contacts[0].normal) * (1 - speedLoss);
	}

//	void Reset()
//	{
//		lastVel = Vector3.zero;
//		
//		StopCoroutine(ScaleUp());
//		
//		transform.position = Vector3.zero;
//		transform.rotation = Quaternion.identity;
//		transform.localScale = Vector3.one * scale;
//		
//		rigidbody2D.velocity = Vector3.zero;
//		rigidbody2D.angularVelocity = 0.0f;
//	}
}