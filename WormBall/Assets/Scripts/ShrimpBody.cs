using UnityEngine;
using System.Collections;
using System.Linq;

public class ShrimpBody : MonoBehaviour 
{
	[SerializeField] int bodyLength;
	[SerializeField] float followSpeed = 1.0f;
	[SerializeField] float scaleAmount = 0.25f;
	[SerializeField] Vector3 offset = Vector3.zero;

	[SerializeField] GameObject bodySegmentPrefab;
	[SerializeField] GameObject tailPrefab;

	Transform[] bodySegments;

	void Awake()
	{
		bodySegments = new Transform[bodyLength + 1];

		for(int i = 0; i < bodyLength; i++)
		{
			GameObject segment = WadeUtils.Instantiate(bodySegmentPrefab, transform.position, transform.rotation);
			segment.transform.parent = transform.parent;

			segment.transform.localScale = Vector3.one * (1.0f - 0.2f * (i + 1));

			SpriteRenderer sr = segment.GetComponent<SpriteRenderer>();
			sr.color = Color.white - new Color(0.1f, 0.1f, 0.1f, 0.0f) * (i + 1);
			sr.sortingLayerID = 0 - (i + 1);

			bodySegments[i] = segment.transform;
		}

		GameObject tail = WadeUtils.Instantiate(tailPrefab, transform.position, transform.rotation);
		tail.transform.parent = transform.parent;
		tail.GetComponentInChildren<SpriteRenderer>().sortingLayerID = bodySegments.Length;
		bodySegments[bodySegments.Length - 1] = tail.transform;
	}

	void Update () 
	{
		for(int i = 0; i < bodySegments.Length; i++)
		{
			Vector3 targetPos = i < 1 ? (Vector3)rigidbody2D.position : bodySegments[i - 1].position;
			Quaternion targetRot = i < 1 ? transform.rotation : bodySegments[i - 1].rotation;

			if(i < bodySegments.Length - 1)
			{
				bodySegments[i].position = Vector3.Lerp(bodySegments[i].position, 
				                                       	targetPos + targetRot * offset, 
				                                        Time.deltaTime * followSpeed);
			}
			else
			{
				bodySegments[i].position = Vector3.Lerp(bodySegments[i].position, 
				                                        targetPos + targetRot * offset, 
				                                        Time.deltaTime * followSpeed * 1.5f);
			}


			bodySegments[i].LookAt(targetPos);
			bodySegments[i].rotation *= Quaternion.Euler(0.0f, 90.0f, 90.0f);
		}
	}
}
