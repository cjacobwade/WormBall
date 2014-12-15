using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawCircle : MonoBehaviour 
{
	float twoPI = 2.0f * Mathf.PI;
	float lastFillAmount = 1.0f;
	[Range (0.0f, 1.0f)] public float fillAmount;
	[SerializeField] int outerVerts = 360;
	[SerializeField] float circleSize = 1.0f;

	[SerializeField] bool debug = false;

	Vector3[] vertices;
	Vector2[] uvs;
	int[] triangles;
	
	void Awake () 
	{
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
