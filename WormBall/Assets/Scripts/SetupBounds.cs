using UnityEngine;
using System.Collections;

public class SetupBounds : SingletonBehaviour<SetupBounds>
{
	public Vector3 worldMin;
	public Vector3 worldMax;

	[SerializeField] GameObject cornerColPrefab;

	void Start () 
	{
		StartCoroutine(SetupBoundsRoutine());
	}

	IEnumerator SetupBoundsRoutine()
	{
		yield return new WaitForSeconds(0.1f);

		Camera cam = Camera.main;
		
		worldMin = cam.ScreenToWorldPoint(new Vector2(0.0f, 0.0f));
		worldMax = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		
		SetupLeft();
		SetupRight();
		SetupTop();
		SetupBottom();
		SetupCornerColliders();
		SetupBallContainer();
	}

	void SetupLeft()
	{
		GameObject leftBounds = new GameObject("LeftBounds", typeof(Rigidbody2D), typeof(BoxCollider2D));
		leftBounds.transform.position = new Vector2(worldMin.x, 0.0f);
		leftBounds.transform.SetParent(transform);
		leftBounds.GetComponent<Rigidbody2D>().isKinematic = true;
		BoxCollider2D leftBox = leftBounds.GetComponent<BoxCollider2D>();
		leftBox.size = new Vector2(leftBox.bounds.size.x, worldMax.y * 2.0f);
		leftBox.center = new Vector2(leftBox.center.x - 0.5f, leftBox.center.y);
	}
		
	void SetupRight()
	{
		GameObject rightBounds = new GameObject("RightBounds", typeof(Rigidbody2D), typeof(BoxCollider2D));
		rightBounds.transform.position = new Vector2(worldMax.x, 0.0f);
		rightBounds.transform.SetParent(transform);
		rightBounds.GetComponent<Rigidbody2D>().isKinematic = true;
		BoxCollider2D rightBox = rightBounds.GetComponent<BoxCollider2D>();
		rightBox.size = new Vector2(rightBox.bounds.size.x, worldMax.y * 2.0f);
		rightBox.center = new Vector2(rightBox.center.x + 0.5f, rightBox.center.y);
	}

	void SetupTop()
	{
		GameObject topBounds = new GameObject("TopBounds", typeof(Rigidbody2D), typeof(BoxCollider2D));
		topBounds.transform.position = new Vector2(0.0f, worldMax.y);
		topBounds.transform.SetParent(transform);
		topBounds.GetComponent<Rigidbody2D>().isKinematic = true;
		BoxCollider2D topBox = topBounds.GetComponent<BoxCollider2D>();
		topBox.size = new Vector2(worldMax.x * 2.0f, topBox.bounds.size.y);
		topBox.center = new Vector2(topBox.center.x, topBox.center.y + 0.5f);
	}

	void SetupBottom()
	{
		GameObject bottomBounds = new GameObject("BottomBounds", typeof(Rigidbody2D), typeof(BoxCollider2D));
		bottomBounds.transform.position = new Vector2(0.0f, worldMin.y);
		bottomBounds.transform.SetParent(transform);
		bottomBounds.GetComponent<Rigidbody2D>().isKinematic = true;
		BoxCollider2D bottomBox = bottomBounds.GetComponent<BoxCollider2D>();
		bottomBox.size = new Vector2(worldMax.x * 2.0f, bottomBox.bounds.size.y);
		bottomBox.center = new Vector2(bottomBox.center.x, bottomBox.center.y - 0.5f);
	}

	void SetupCornerColliders()
	{
		Vector3 topLeft = worldMax;
		topLeft.x *= -1;

		Vector3 bottomRight = worldMin;
		bottomRight.x *= -1;

		GameObject topLeftCol = WadeUtils.Instantiate(	cornerColPrefab, 
		                      							topLeft + (Vector3.right - Vector3.up) * 2.0f, 
		                      							Quaternion.identity);
		topLeftCol.name = "TopLeftCorner";
		topLeftCol.transform.SetParent(transform);

		GameObject topRightCol = WadeUtils.Instantiate(	cornerColPrefab, 
		                      							worldMax + (-Vector3.right - Vector3.up) * 2.0f, 
		                      							Quaternion.Euler(0.0f, 0.0f, 270.0f));
		topRightCol.name = "TopRightCorner";
		topRightCol.transform.SetParent(transform);

		GameObject bottomRightCol = WadeUtils.Instantiate(	cornerColPrefab, 
		                                                  bottomRight + (-Vector3.right + Vector3.up) * 2.0f, 
		                                                  Quaternion.Euler(0.0f, 0.0f, 180.0f));
		bottomRightCol.name = "BottomRightCorner";
		bottomRightCol.transform.SetParent(transform);

		GameObject bottomLeftCol = WadeUtils.Instantiate(	cornerColPrefab, 
		                      								worldMin + (Vector3.right + Vector3.up) * 2.0f, 
		                      								Quaternion.Euler(0.0f, 0.0f, 90.0f));
		bottomLeftCol.name = "BottomLeftCorner";
		bottomLeftCol.transform.SetParent(transform);
	}

	void SetupBallContainer()
	{
		GameObject ballContainer = new GameObject("BallContainer", typeof(BoxCollider2D));
		ballContainer.transform.position = Vector3.zero;
		ballContainer.transform.SetParent(transform);
		BoxCollider2D ballBox = ballContainer.GetComponent<BoxCollider2D>();
		ballBox.isTrigger = true;
		ballBox.size = new Vector2(worldMax.x * 2.0f, worldMax.y * 2.0f);
		ballBox.center = Vector3.zero;
	}

	public bool IsInBounds(Vector3 pos)
	{
		if(pos.x > worldMax.x || pos.y > worldMax.y || 
		   pos.x < worldMin.x || pos.y < worldMin.y)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}
