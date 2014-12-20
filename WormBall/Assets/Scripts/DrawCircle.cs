using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCircle : MonoBehaviour 
{
	float twoPI = 2.0f * Mathf.PI;
	float lastFillAmount = 1.0f;
	[Range (0.0f, 1.0f)] public float fillAmount;
	[SerializeField] int outerVerts = 360;

	float initCircleSize;
	public float circleSize = 1.0f;

	[SerializeField] AnimationCurve scaleCurve;

	[SerializeField] bool debug = false;

	Vector3[] vertices;
	Vector2[] uvs;
	int[] triangles;
	
	void Awake () 
	{
		initCircleSize = circleSize;
	}

	void Update () 
	{
		if(fillAmount != lastFillAmount)
		{
			CreateMesh();
			lastFillAmount = fillAmount;
		}
		else
		{
			UpdateMesh();
		}
	}

	public void PingPongSize(float toSize, float pingTime)
	{
		StopAllCoroutines();
		StartCoroutine(PingPongSizeRoutine(toSize, pingTime));
	}

	IEnumerator PingPongSizeRoutine(float toSize, float pingTime)
	{
		float fromSize = circleSize;
		float targetSize = toSize;
		float pingTimer = 0f;

		float tmpSize;

		int i = 0;

		while(true)
		{
			while(pingTimer < pingTime)
			{
				circleSize = Mathf.Lerp(fromSize, targetSize, scaleCurve.Evaluate(pingTimer/pingTime));

				pingTimer += Time.deltaTime;

				yield return 0;
			}

			if(i < 1)
			{
				fromSize = initCircleSize;
			}

			tmpSize = fromSize;
			fromSize = targetSize;
			targetSize = tmpSize;

			i++;
			pingTimer = 0f;
		}

	}

	public void ReturnToSize(float returnTime)
	{
		StopAllCoroutines();
		StartCoroutine(ReturnToSizeRoutine(returnTime));
	}

	IEnumerator ReturnToSizeRoutine(float returnTime)
	{
		float returnTimer = 0f;
		float initSize = circleSize;

		while(returnTimer < returnTime)
		{
			circleSize = Mathf.Lerp(initSize, initCircleSize, scaleCurve.Evaluate(returnTimer/returnTime));
			
			returnTimer += Time.deltaTime;
			yield return 0;
		}

		yield return 0;
	}

	void SetupMeshData()
	{
		vertices = new Vector3[outerVerts + 2];
		uvs = new Vector2[outerVerts + 2];

		List<int> tris = new List<int>();
		vertices[0] = Vector3.zero;



		for(int i = 1; i < outerVerts + 2; i++)
		{
			vertices[i] = new Vector3(Mathf.Cos((float)i * twoPI * fillAmount/(float)outerVerts) * circleSize, 
			                          Mathf.Sin((float)i * twoPI * fillAmount/(float)outerVerts) * circleSize, 
			                          0.0f);

			if(i < outerVerts + 1)
			{
				tris.Add(0);
				tris.Add(i);
				tris.Add(i + 1);
			}
		}

		triangles = tris.ToArray();
	}

	void CreateMesh()
	{
		SetupMeshData();

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.RecalculateNormals();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	void UpdateMesh()
	{
		SetupMeshData();

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.RecalculateNormals();
	}

	void OnGUI()
	{
		for(int i = 0; debug && i < vertices.Length; i++)
		{
			Vector3 screenVec = Camera.main.WorldToScreenPoint(Quaternion.Euler(0f, 0f, 90f) * vertices[i]);
			GUI.Label(new Rect(screenVec.x - 3.0f, Screen.height - (screenVec.y + 3.0f), 20.0f, 20.0f), i.ToString());
		}
	}
}
